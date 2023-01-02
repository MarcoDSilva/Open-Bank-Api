using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;
public interface IUserRepository
{
    Task<User> CreateUserAsync(User createUserRequest);
    Task<User?> GetUserAsync(string username);
    List<User> GetAllUsers(); // high chance this needs a DTO
}

