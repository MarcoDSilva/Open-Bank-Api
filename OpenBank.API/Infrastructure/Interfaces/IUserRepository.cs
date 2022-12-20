using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Infrastructure.Interfaces;
public interface IUserRepository
{
    Task<CreateUserRequest> CreateUser(CreateUserRequest createUserRequest);
    bool IsUsernameAvailable(string username);
    int IsLoginValid(LoginUserRequest login);
    IEnumerable<User> GetAllUsers(); // high chance this needs a DTO
}

