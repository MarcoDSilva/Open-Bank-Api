using OpenBank.API.DTO;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace OpenBank.API.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public UserRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<CreateUserRequest> CreateUser(CreateUserRequest createUserRequest)
    {
        PasswordHasher<string> pwHasher = new PasswordHasher<string>();

        User requestToUser = new User()
        {
            Email = createUserRequest.Email,
            FullName = createUserRequest.FullName,
            Password = pwHasher.HashPassword(createUserRequest.Username, createUserRequest.Password),
            UserName = createUserRequest.Username,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var inserted = await _openBankApiDbContext.Users.AddAsync(requestToUser);
            var save = await _openBankApiDbContext.SaveChangesAsync();

            return createUserRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error {0}", e.Message); //Log erro
            throw new Exception("Error while creating the user");
        }
    }

    public IEnumerable<User> GetAllUsers()
    {
        var userList = _openBankApiDbContext.Users.ToList();
        return userList;
    }

    /// <summary>
    /// Validating the existance of the username that is being inserted
    /// </summary>
    /// <returns> true if the username is free
    public bool IsUsernameAvailable(string username)
    {
        var userList = _openBankApiDbContext.Users.ToList();
        var exists = userList.Find(m => m.UserName.ToLower().Equals(username.ToLower()));

        return string.IsNullOrEmpty(exists?.UserName);
    }

    public bool IsLoginValid(LoginUserRequest login)
    {
        try
        {
            var existentUser = _openBankApiDbContext.Users
                                    .ToList()
                                    .Find(user => user.UserName.ToLower() == login.UserName.ToLower());

            if (string.IsNullOrWhiteSpace(existentUser?.UserName)) return false;

            PasswordHasher<string> pwHasher = new PasswordHasher<string>();
            int validUser = (int)pwHasher.VerifyHashedPassword(
                                    login.UserName, existentUser.Password, login.Password
                                );

            return validUser != 0;
        }
        catch (Exception e)
        {
            throw new Exception($"Server issues: {e.Message}" );
        }
    }

}

