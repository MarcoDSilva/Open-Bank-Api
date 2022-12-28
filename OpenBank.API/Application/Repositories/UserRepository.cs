using OpenBank.API.Application.DTO;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OpenBank.API.Application.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public UserRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<CreateUserResponse> CreateUser(User createUser)
    {
        var inserted = await _openBankApiDbContext.Users.AddAsync(createUser);
        var save = await _openBankApiDbContext.SaveChangesAsync();

        return UserToCreationResponseDTO(inserted.Entity);
    }

    public List<User> GetAllUsers()
    {
        var userList = _openBankApiDbContext.Users.ToList();
        return userList;
    }

    public async Task<User?> GetUser(string username)
    {
        List<User> userList = await _openBankApiDbContext.Users.ToListAsync();
        User? user = userList.Find(m => m.UserName.ToLower().Equals(username.ToLower()));

        return user;
    }

    private CreateUserResponse UserToCreationResponseDTO(User user)
    {
        return new CreateUserResponse()
        {
            CreatedAt = user.Created_at,
            Email = user.Email,
            FullName = user.FullName,
            Id = user.Id,
            PasswordChangedAt = user.Created_at,
            Username = user.UserName
        };
    }
}

