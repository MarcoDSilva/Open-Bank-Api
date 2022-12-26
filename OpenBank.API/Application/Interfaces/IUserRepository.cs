using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;
public interface IUserRepository
{
    Task<CreateUserRequest> CreateUser(CreateUserRequest createUserRequest);
    bool IsUsernameAvailable(string username);
    int IsLoginValid(LoginUserRequest login);
    IEnumerable<User> GetAllUsers(); // high chance this needs a DTO
}

