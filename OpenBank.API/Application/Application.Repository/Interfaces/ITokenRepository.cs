using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Interfaces;

public interface ITokenRepository
{
    Task<Token?> AddTokenAsync(Token token);
    Task<Token> GetTokenAsync(string token, string jti);
    Token? UpdateToken(Token token);
    Task<bool> SaveAsync();
}