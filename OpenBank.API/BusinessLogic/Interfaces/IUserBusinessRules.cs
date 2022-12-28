using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessLogic.Interfaces;
public interface IUserBusinessRules
{
    Task<CreateUserResponse> CreateUser(CreateUserRequest createUserRequest);
    List<User> GetAllUsers(); 
    Task<bool> IsUsernameAvailable(string username);
    Task<int> IsLoginValid(LoginUserRequest login);
}

