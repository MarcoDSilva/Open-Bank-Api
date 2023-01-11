using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Interfaces;
public interface IUserRepository
{
    Task<User> CreateUserAsync(User createUserRequest);
    Task<User?> GetUserAsync(string username);
    List<User> GetAllUsers(); // high chance this needs a DTO
}

