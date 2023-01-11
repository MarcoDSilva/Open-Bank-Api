using OpenBank.Api.Data;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.API.Application.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OpenBank.API.Application.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public UserRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<User> CreateUserAsync(User createUser)
    {
        var inserted = await _openBankApiDbContext.Users.AddAsync(createUser);
        var save = await _openBankApiDbContext.SaveChangesAsync();

        return inserted.Entity;
    }

    public List<User> GetAllUsers()
    {
        var userList = _openBankApiDbContext.Users.ToList();
        return userList;
    }

    public async Task<User?> GetUserAsync(string username)
    {
        var userList = await _openBankApiDbContext.Users.ToListAsync();
        var user = userList.Find(m => m.UserName.ToLower().Equals(username.ToLower()));

        return user;
    }
}

