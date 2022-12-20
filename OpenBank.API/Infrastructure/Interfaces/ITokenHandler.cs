using OpenBank.API.Infrastructure.DTO;

namespace OpenBank.API.Infrastructure.Interfaces;

public interface ITokenHandler
{
    Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginRequest, int userId);
    int GetUserIdByToken(string token);
}