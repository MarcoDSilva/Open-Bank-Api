using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Interfaces;

public interface ITokenRepository
{
    Task<RefreshToken?> AddTokenAsync(RefreshToken token);
    Task<RefreshToken> GetTokenAsync(string token, string jti);
    RefreshToken? UpdateToken(RefreshToken token);
    Task<bool> SaveAsync();
}