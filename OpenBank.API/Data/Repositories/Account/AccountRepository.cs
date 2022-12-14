using OpenBank.Api.Data;
using OpenBank.API.Models.Entities;

namespace OpenBank.API.Data;

public class AccountRepository : IAccountRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public AccountRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public Task<CreateAccountRequest> CreateAccount(CreateAccountRequest createAccount)
    {
        throw new NotImplementedException();
    }

    public Task<Account> GetAccountById(int accountId)
    {
        throw new NotImplementedException();
    }
    public Task<List<Account>> GetAccounts(User user)
    {
        throw new NotImplementedException();
    }

}