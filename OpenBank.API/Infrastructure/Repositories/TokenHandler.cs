using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Infrastructure.Interfaces;
using System.Data.Common;

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
        DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);

        List<Claim> claims = new List<Claim>()
        {
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.Expiration, expirationDate.ToString()),
            new Claim(ClaimTypes.NameIdentifier, loginUserRequest.UserName)
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
}