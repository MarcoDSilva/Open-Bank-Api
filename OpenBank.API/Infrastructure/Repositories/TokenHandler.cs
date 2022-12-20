using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Infrastructure.Repositories;

public class TokenHandler : ITokenHandler
{
    private readonly IConfiguration _configuration;

    public TokenHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginUserRequest, int userId)
    {
        string sessionId = Guid.NewGuid().ToString();
        DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Expiration, expirationDate.ToString()),
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, loginUserRequest.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
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
            SessionId = sessionId
        };

        return Task.FromResult(response);
    }
}