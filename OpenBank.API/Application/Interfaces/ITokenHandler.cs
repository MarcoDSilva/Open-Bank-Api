using OpenBank.API.Application.DTO;

namespace OpenBank.API.Application.Interfaces;

public interface ITokenHandler
{
    Task<LoginUserResponse> CreateTokenAsync(LoginUserRequest loginRequest, int userId);
    int GetUserIdByToken(string token);
}