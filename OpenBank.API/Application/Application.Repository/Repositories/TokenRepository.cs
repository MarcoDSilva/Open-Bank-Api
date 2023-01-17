using OpenBank.Api.Data;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.API.Application.Repository.Interfaces;

namespace OpenBank.API.Application.Repository.Repositories;

public class TokenRepository : ITokenRepository
{
    public Task<Token> AddTokenAsync(Token token)
    {
        throw new NotImplementedException();
    }

    public Task<Token> GetTokenAsync(string token, string jti)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Token> UpdateTokenAsync(Token token)
    {
        throw new NotImplementedException();
    }
}