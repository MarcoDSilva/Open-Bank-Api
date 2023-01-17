using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Interfaces;

public interface ITokenRepository
{
    Task<Token> GetTokenAsync(string token, string jti);
    Task<Token> AddTokenAsync(Token token);
    Task<Token> UpdateTokenAsync(Token token);
    Task<bool> SaveAsync();

}