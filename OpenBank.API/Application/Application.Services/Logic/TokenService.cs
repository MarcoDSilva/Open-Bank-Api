using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;

namespace OpenBank.API.Application.Repository.Repositories;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private const int ExpirationTime = 5;
    private const int RefreshExpirationTime = 60;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginUserRequest, int userId)
    {
        DateTime expirationDate = DateTime.UtcNow.AddMinutes(ExpirationTime);
        DateTime refreshExpirationDate = DateTime.UtcNow.AddMinutes(RefreshExpirationTime);

        var claims = new List<Claim>()
        {
            new ("userId", userId.ToString()),
            new (ClaimTypes.Expiration, expirationDate.ToString()),
            new (ClaimTypes.NameIdentifier, loginUserRequest.UserName),
            new (ClaimTypes.Expiration, expirationDate.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var refreshToken = Guid.NewGuid().ToString();
        claims.Add(new("refreshToken", refreshToken));

        var token = GenerateNewToken(claims, expirationDate, credentials);
        string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new LoginUserResponse()
        {
            AcessToken = generatedToken,
            AcessTokenExpires = expirationDate.ToString(),
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshExpirationDate.ToString(),
            SessionId = ""
        };

        return Task.FromResult(response);
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


    public bool RevokeToken()
    {
        return false;
    }

    public void RenewToken(string tokenWithBearer)
    {

    }
}