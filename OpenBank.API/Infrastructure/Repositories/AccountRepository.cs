using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.DTO;
using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public AccountRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    /* Control test*/
    public async Task<CreateAccountRequest> CreateAccount(int idUser, CreateAccountRequest createAccount)
    {
        Account account = new Account()
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

        try
        {
            var account = accountList.Find(acc => acc.Id == accountId);

            return account;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error {0}", e.Message); //Log erro
            throw new Exception("Error while obtaining the movements of the account");
        }
    }

    public async Task<List<Account>> GetAccounts(int userId)
    {
        var existantAccounts = await _openBankApiDbContext.Accounts.ToListAsync();
        try
        {
            var accounts = existantAccounts.FindAll(acc => acc.UserId == userId).ToList();

            return accounts;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error {0}", e.Message); //Log erro
            throw new Exception("Error while obtaining the accounts");
        }
    }

    public async Task<AccountMovim> GetAccountMovements(Account account)
    {
        var existantMovements = await _openBankApiDbContext.Movim.ToListAsync();

        try
        {
            var movements = existantMovements.FindAll(mov => mov.AccountId == account.Id).ToList();

            List<MovimResponse> movementsToDTO = new List<MovimResponse>();

            foreach (Movim movement in movements)
            {
                movementsToDTO.Add(new MovimResponse()
                {
                    Id = movement.Id,
                    Amount = movement.Balance,
                    Created_at = movement.Created_at,
                    OperationType = movement.OperationType
                });
            }

            AccountMovim accountMovim = new AccountMovim()
            {
                Account = account,
                Movimentos = movementsToDTO,
            };

            return accountMovim;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

}