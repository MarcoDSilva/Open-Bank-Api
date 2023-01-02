using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;
public interface IUserRepository
{
    Task<CreateUserResponse> CreateUser(User createUserRequest);
    Task<User?> GetUser(string username);
    List<User> GetAllUsers(); // high chance this needs a DTO
}

