using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Models.Entities;

namespace OpenBank.API.Data;

public class UserRepository : IUserRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public UserRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<CreateUserRequest> CreateUser(CreateUserRequest createUserRequest)
    {
        var requestToUser = new User()
        {
            Email = createUserRequest.Email,
            FullName = createUserRequest.FullName,
            Password = createUserRequest.Password,
            UserName = createUserRequest.Username
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
        var exists = userList.Find(m => m.UserName.Equals(username));

        return string.IsNullOrEmpty(exists?.UserName);
    }

}

