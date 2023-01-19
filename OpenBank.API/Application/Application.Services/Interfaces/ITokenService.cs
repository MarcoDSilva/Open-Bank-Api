using OpenBank.API.Application.DTO;

namespace OpenBank.API.Application.Repository.Interfaces;

public interface ITokenService
{
    Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginRequest, string userId);
    int GetUserIdFromToken(string token);
    Task<bool> RevokeToken(string token);
    Task<LoginUserResponse> RenewTokenAsync(string token);
}