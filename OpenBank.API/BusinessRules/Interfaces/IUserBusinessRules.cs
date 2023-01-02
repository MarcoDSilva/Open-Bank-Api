using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessRules.Interfaces;
public interface IUserBusinessRules
{
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest);
    List<User> GetAllUsers(); 
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<int> GetUserIdAsync(LoginUserRequest login);
}

