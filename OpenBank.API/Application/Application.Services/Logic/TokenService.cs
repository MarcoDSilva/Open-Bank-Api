using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Repositories;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private const int ExpirationTime = 5;
    private const int RefreshExpirationTime = 60;

    public TokenService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginUserRequest, int userId)
    {
        DateTime expirationDate = DateTime.UtcNow.AddMinutes(ExpirationTime);
        DateTime refreshExpirationDate = DateTime.UtcNow.AddMinutes(RefreshExpirationTime);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var refreshToken = Guid.NewGuid().ToString();
        var Jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>()
        {
            new ("userId", userId.ToString()),
            new (ClaimTypes.Expiration, expirationDate.ToString()),
            new (ClaimTypes.NameIdentifier, loginUserRequest.UserName),
            new("refreshToken", refreshToken),
            new("refreshExpiration", refreshExpirationDate.ToString()),
            new("jti", Jti)
        };

        var token = GenerateNewToken(claims, expirationDate, credentials);
        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var registerToken = new Token()
        {
            Created_at = DateTime.UtcNow,
            ExpirationTime = expirationDate,
            Jti = Jti,
            RefreshToken = refreshToken,
            UsedDate = null,
            UserId = userId
        };

        var savedToken = await _unitOfWork.tokenRepository.AddTokenAsync(registerToken);
        if (savedToken?.Id <= 0) throw new Exception("Could not create token permission");

        var response = new LoginUserResponse()
        {
            AcessToken = accessToken,
            AcessTokenExpires = expirationDate.ToString(),
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshExpirationDate.ToString(),
            SessionId = ""
        };

        return response;
    }

    public int GetUserIdByToken(string tokenWithBearer)
    {
        if (string.IsNullOrWhiteSpace(tokenWithBearer)) return 0; // validations against a possible empty header token

        try
        {
            string tokenWithoutBearer = tokenWithBearer.Replace("Bearer ", "");
            var claims = new JwtSecurityToken(tokenWithoutBearer).Claims;

            string user = claims.First(c => c.Type == "userId").Value;

            return int.Parse(user);
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Error at GetUserIdByToken with the following message: {e.Message}");
            throw new Exception("Error while validating user");
        }
    }

    private JwtSecurityToken GenerateNewToken(List<Claim> claims, DateTime expirationDate, SigningCredentials credentials)
    {
        return new JwtSecurityToken(
                   _configuration["Jwt:Issuer"],
                   _configuration["Jwt:Audience"],
                   claims,
                   expires: expirationDate,
                   signingCredentials: credentials
               );
    }


    public bool RevokeToken(string tokenWithBearer)
    {
        return false;
    }

    public void RenewToken(string tokenWithBearer)
    {
        return;
    }
}