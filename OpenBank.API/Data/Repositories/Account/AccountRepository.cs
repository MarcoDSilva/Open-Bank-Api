using Microsoft.EntityFrameworkCore;
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

    /* Control test*/
    public async Task<CreateAccountRequest> CreateAccount(int idUser, CreateAccountRequest createAccount)
    {
        var account = new Account()
        {
            Balance = createAccount.Amount,
            Created_at = DateTime.UtcNow,
            Currency = "EUR",
            UserId = idUser
        };

        try
        {
            var inserted = await _openBankApiDbContext.Accounts.AddAsync(account);
            var save = await _openBankApiDbContext.SaveChangesAsync();

            return createAccount;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error {0}", e.Message); //Log erro
            throw new Exception("Error while creating the user");
        }
    }

    public async Task<Account> GetAccountById(int accountId)
    {
        var accountList = await _openBankApiDbContext.Accounts.ToListAsync();
        var account = accountList.Find(acc => acc.Id == accountId);

        return account;
    }
    public Task<List<Account>> GetAccounts(int userId)
    {
        throw new NotImplementedException();
    }

}