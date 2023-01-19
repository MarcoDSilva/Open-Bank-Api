using OpenBank.Api.Data;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.API.Application.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OpenBank.API.Application.Repository.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public TokenRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<RefreshToken?> AddTokenAsync(RefreshToken token)
    {
        var saved = await _openBankApiDbContext.AddAsync(token);
        return saved.Entity;
    }

    public async Task<RefreshToken> GetTokenAsync(string refreshToken, string jti)
    {
        var tokenList = await _openBankApiDbContext.Tokens.ToListAsync();
        var token = tokenList.FirstOrDefault(t => t.Token.Equals(refreshToken) && t.Jti.Equals(jti));

        return token;
    }

    public async Task<bool> SaveAsync()
    {
        var result = await _openBankApiDbContext.SaveChangesAsync();
        return result > 0;
    }

    public RefreshToken? UpdateToken(RefreshToken token)
    {
        var updated = _openBankApiDbContext.Tokens.Update(token);
        return updated.Entity;
    }
}