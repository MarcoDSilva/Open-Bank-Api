using OpenBank.Api.Data;
using OpenBank.API.Application.DTO;
using OpenBank.API.BusinessLogic.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessLogic;

public class UserBusinessRules : IUserBusinessRules
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public UserBusinessRules(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public Task<CreateUserRequest> CreateUser(CreateUserRequest createUserRequest)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public int IsLoginValid(LoginUserRequest login)
    {
        throw new NotImplementedException();
    }

    public bool IsUsernameAvailable(string username)
    {
        throw new NotImplementedException();
    }
}