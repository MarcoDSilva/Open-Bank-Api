using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;

namespace OpenBank.API.Application.Repository.Repositories;

public class TokenHandler : ITokenHandler
{
    private readonly IConfiguration _configuration;
    private const int ExpirationTime = 5;

    public TokenHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginUserRequest, int userId)
    {
        DateTime expirationDate = DateTime.UtcNow.AddMinutes(ExpirationTime);

        var claims = new List<Claim>()
        {
            new ("userId", userId.ToString()),
            new (ClaimTypes.Expiration, expirationDate.ToString()),
            new (ClaimTypes.NameIdentifier, loginUserRequest.UserName)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expirationDate,
            signingCredentials: credentials
        );

        string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

        LoginUserResponse response = new LoginUserResponse()
        {
            AcessToken = generatedToken,
            AcessTokenExpires = expirationDate.ToString(),
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
}