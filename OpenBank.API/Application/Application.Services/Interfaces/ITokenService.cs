using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Interfaces;

public interface ITokenService
{
    Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginRequest, string userId);
    int GetUserIdFromToken(string token);
    Task<bool> RevokeToken(string token);
    Task<RefreshToken> ValidateToken(string token);
    Task<LoginUserResponse> RenewTokenAsync(RefreshToken token);
}