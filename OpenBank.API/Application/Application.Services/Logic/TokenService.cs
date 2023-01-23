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
        var data = CreateDataForClaims(userId, loginUserRequest.UserName);
        var claims = CreateClaims(data);
        var accessToken = CreateToken(claims, DateTimeOffset.Parse(data["expirationDate"]).UtcDateTime);

        var registerToken = new RefreshToken()
        {
            Created_at = DateTime.UtcNow,
            ExpirationTime = DateTimeOffset.Parse(data["expirationDate"]).UtcDateTime,
            Jti = data["jti"],
            Token = data["refreshToken"],
            UsedDate = null,
            UserId = int.Parse(userId)
        };

        var savedToken = await _unitOfWork.tokenRepository.AddTokenAsync(registerToken);

        if (savedToken != registerToken) throw new Exception("Could not create token permission");

        bool isSaved = await _unitOfWork.tokenRepository.SaveAsync();
        if (!isSaved) throw new Exception("Could not create token permission");

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

    public int GetUserIdFromToken(string tokenWithBearer)
    {
        if (string.IsNullOrWhiteSpace(tokenWithBearer)) return 0; // validations against a possible empty header token

        try
        {
            var claims = GetClaims(tokenWithBearer);
            string user = claims.First(c => c.Type == "userId").Value;

            return int.Parse(user);
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Error at GetUserIdByToken with the following message: {e.Message}");
            throw new Exception("Error while validating user");
        }
    }


    public async Task<bool> RevokeToken(string tokenWithBearer)
    {
        var claims = GetClaims(tokenWithBearer);
        var claimsValues = GetClaimValues(claims);

        var refreshToken = await _unitOfWork.tokenRepository.GetTokenAsync(claimsValues["refreshTokenClaim"], claimsValues["jti"]);

        if (refreshToken is null || refreshToken.UserId.ToString() != claimsValues["userId"])
            throw new InvalidUserAccessException("Invalid Token");

        refreshToken.UsedDate = DateTime.UtcNow;

        var updatedToken = _unitOfWork.tokenRepository.UpdateToken(refreshToken);
        bool isSaved = await _unitOfWork.tokenRepository.SaveAsync();

        return isSaved;
    }

    public async Task<RefreshToken> ValidateToken(string tokenWithBearer)
    {
        var claims = GetClaims(tokenWithBearer);
        var values = GetClaimValues(claims);

        if (string.IsNullOrEmpty(values["userId"]) || int.Parse(values["userId"]) <= 0) throw new InvalidUserAccessException("User not found.");
        if (string.IsNullOrEmpty(values["refreshTokenClaim"]) || string.IsNullOrEmpty(values["jti"])) throw new IllegalTokenException("Tokens not found.");
        if (string.IsNullOrEmpty(values["refreshExpirationClaim"]) || DateTimeOffset.Parse(values["refreshExpirationClaim"]).UtcDateTime <= DateTime.UtcNow)
            throw new ExpiredTokenException("Expired token.");

        var refreshToken = await _unitOfWork.tokenRepository.GetTokenAsync(values["refreshTokenClaim"], values["jti"]);
        if (refreshToken is null || refreshToken.UserId.ToString() != values["userId"]) throw new InvalidUserAccessException("Invalid Token");

        bool isRefreshTokenValid = refreshToken.ExpirationTime >= DateTime.UtcNow && refreshToken.UsedDate is null;
        if (!isRefreshTokenValid) throw new ExpiredTokenException("Expired token.");

        return refreshToken;
    }

    public async Task<LoginUserResponse> RenewTokenAsync(RefreshToken token)
    {
        var updatedToken = _unitOfWork.tokenRepository.UpdateToken(token);
        if (updatedToken != token) throw new Exception("Could not update token");

        var claimData = CreateDataForClaims(token.UserId.ToString());
        var newClaims = CreateClaims(claimData);
        var accessToken = CreateToken(newClaims, DateTimeOffset.Parse(claimData["expirationDate"]).UtcDateTime);

        var newTokenInsertion = _unitOfWork.tokenRepository.AddTokenAsync(new RefreshToken()
        {
            Created_at = DateTime.UtcNow,
            ExpirationTime = DateTimeOffset.Parse(claimData["refreshExpirationDate"]).UtcDateTime,
            Token = claimData["refreshToken"],
            Jti = claimData["jti"],
            UsedDate = null,
            UserId = token.UserId
        });

        bool isSaved = await _unitOfWork.tokenRepository.SaveAsync();
        if (!isSaved) throw new Exception("Could not save the token");

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


    private Dictionary<string, string> GetClaimValues(IEnumerable<Claim> claims)
    {
        return new Dictionary<string, string>()
        {
            {"userId", claims.First(c => c.Type == "userId").Value },
            {"refreshTokenClaim", claims.First(c => c.Type == "refreshToken").Value },
            {"refreshExpirationClaim", claims.First(c => c.Type == "refreshExpiration").Value },
            {"jti", claims.First(c => c.Type == "jti").Value },
            {"username", claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value }
        };
    }

    private IEnumerable<Claim> GetClaims(string tokenWithBearer)
    {
        string token = tokenWithBearer.Replace("Bearer ", "");
        var claims = new JwtSecurityToken(token).Claims;

        return claims;
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

    private Dictionary<string, string> CreateDataForClaims(string userId, string username = "")
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