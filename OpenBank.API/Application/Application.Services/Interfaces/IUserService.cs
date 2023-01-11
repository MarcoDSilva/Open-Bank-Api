using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Interfaces;
public interface IUserService
{
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest);
    List<User> GetAllUsers(); 
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<int> GetUserIdAsync(string username, string password);
}

