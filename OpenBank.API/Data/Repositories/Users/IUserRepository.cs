using OpenBank.API.Models.Entities;

namespace OpenBank.API.Data;

public interface IUserRepository
{
    Task<CreateUserRequest> CreateUser(CreateUserRequest createUserRequest);
    bool IsUsernameAvailable(string username);
    IEnumerable<User> GetAllUsers(); // high chance this needs a DTO
}

