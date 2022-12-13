using System.ComponentModel;
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

    public CreateUserRequest CreateUser(CreateUserRequest createUserRequest)
    {
        var requestToUser = new User()
        {
            Email = createUserRequest.Email,
            FullName = createUserRequest.FullName,
            Password = createUserRequest.Password,
            UserName = createUserRequest.Username
        };

        //var inserted = _openBankApiDbContext.Users.Add(requestToUser);
        return createUserRequest;        
        //return inserted.IsKeySet == true ? createUserRequest : new CreateUserRequest();
    }
}

