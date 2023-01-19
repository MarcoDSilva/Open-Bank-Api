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

    public async Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginUserRequest, string userId)
    {
        var data = DataForClaims(userId, loginUserRequest.UserName);
        var claims = CreateClaims(data);
        var accessToken = CreateToken(claims, DateTimeOffset.Parse(data["expirationDate"]).UtcDateTime);

        var registerToken = new Token()
        {
            Created_at = DateTime.UtcNow,
            ExpirationTime = DateTimeOffset.Parse(data["expirationDate"]).UtcDateTime,
            Jti = data["jti"],
            RefreshToken = data["refreshToken"],
            UsedDate = null,
            UserId = int.Parse(userId)
        };

        var savedToken = await _unitOfWork.tokenRepository.AddTokenAsync(registerToken);
        if (savedToken?.Id <= 0) throw new Exception("Could not create token permission");

        var response = new LoginUserResponse()
        {
            AcessToken = accessToken,
            AcessTokenExpires = data["expirationDate"],
            RefreshToken = data["refreshToken"],
            RefreshTokenExpires = data["refreshExpirationDate"],
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


    public bool RevokeToken(string tokenWithBearer)
    {
        return false;
    }

    public async Task<LoginUserResponse> RenewTokenAsync(string tokenWithBearer)
    {
        if (string.IsNullOrWhiteSpace(tokenWithBearer)) throw new InvalidTokenException();

        string token = tokenWithBearer.Replace("Bearer ", "");
        var claims = new JwtSecurityToken(token).Claims;

        // get claim values
        string userId = claims.First(c => c.Type == "userId").Value;
        string refreshTokenClaim = claims.First(c => c.Type == "refreshToken").Value;
        string refreshExpirationClaim = claims.First(c => c.Type == "refreshExpiration").Value;
        string jtiClaim = claims.First(c => c.Type == "jti").Value;
        string username = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        if (string.IsNullOrEmpty(userId) || int.Parse(userId) <= 0) throw new InvalidTokenException();
        if (string.IsNullOrEmpty(refreshTokenClaim) || string.IsNullOrEmpty(jtiClaim)) throw new InvalidTokenException();
        if (string.IsNullOrEmpty(refreshExpirationClaim) || DateTimeOffset.Parse(refreshExpirationClaim).UtcDateTime <= DateTime.UtcNow)
            throw new InvalidTokenException();

        // validate refresh token on db with jti and validation
        var refreshToken = await _unitOfWork.tokenRepository.GetTokenAsync(refreshTokenClaim, jtiClaim);
        if (refreshToken is null || refreshToken.UserId.ToString() != userId) throw new InvalidTokenException();

        bool isRefreshTokenValid = refreshToken.ExpirationTime >= DateTime.UtcNow && refreshToken.UsedDate == null;

        if (!isRefreshTokenValid) throw new InvalidTokenException();

        var claimData = DataForClaims(userId, username);
        var newClaims = CreateClaims(claimData);

        var accessToken = CreateToken(newClaims, DateTimeOffset.Parse(claimData["expirationDate"]).UtcDateTime);

        // update o refreshtoken original para marcar como usado
        // inserir novo registo na bd
        var response = new LoginUserResponse()
        {
            AcessToken = accessToken,
            AcessTokenExpires = claimData["expirationDate"],
            RefreshToken = claimData["refreshToken"],
            RefreshTokenExpires = claimData["refreshExpirationDate"],
            SessionId = ""
        };

        return response;
    }

    private string CreateToken(List<Claim> claims, DateTime expirationDate)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
                   _configuration["Jwt:Issuer"],
                   _configuration["Jwt:Audience"],
                   claims,
                   expires: expirationDate,
                   signingCredentials: credentials
               );

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return token;
    }

    private List<Claim> CreateClaims(Dictionary<string, string> values)
    {
        return new List<Claim>()
        {
            new ("userId", values["userId"]),
            new (ClaimTypes.Expiration, values["expirationDate"]),
            new (ClaimTypes.NameIdentifier, values["username"]),
            new ("refreshToken", values["refreshToken"]),
            new ("refreshExpiration", values["refreshExpirationDate"]),
            new ("jti", values["jti"])
        };
    }

    private Dictionary<string, string> DataForClaims(string userId, string username)
    {
        return new Dictionary<string, string>()
        {
            {"userId", userId},
            {"username", username},
            {"expirationDate", DateTime.UtcNow.AddMinutes(ExpirationTime).ToString() },
            {"refreshExpirationDate" , DateTime.UtcNow.AddMinutes(RefreshExpirationTime).ToString()},
            {"refreshToken", Guid.NewGuid().ToString()},
            {"jti", Guid.NewGuid().ToString()},
        };
    }
}