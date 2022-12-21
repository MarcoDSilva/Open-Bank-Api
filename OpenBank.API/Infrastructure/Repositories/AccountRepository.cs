using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public AccountRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    /// <summary>
    /// Creates a new account for the user
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with the request back if succeded the creation</returns>
    /// </summary>
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

    /// <summary>
    /// Gets the account by the selected id
    /// <exception>Exception in case something fails | Forbidden account in case the user is not the owner of the account </exception>
    /// <returns>A Task with the account if the account was found</returns>
    /// </summary>
    public async Task<Account> GetAccountById(int accountId, int userId)
    {
        List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();

        Account? account = accountList.Find(acc => acc.Id == accountId);

        if (account?.UserId != userId)
            throw new ForbiddenAccountAccessException("You have no permissions to see this account.");

        return account;
    }


    /// <summary>
    /// Gets all the accounts belonging to the user
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with a list of accounts</returns>
    /// </summary>
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

    /// <summary>
    /// Gets all the account movements belonging to the account
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with the account movements</returns>
    /// </summary>
    public async Task<AccountMovim> GetAccountMovements(Account account)
    {
        var existantMovements = await _openBankApiDbContext.Movim.ToListAsync();

        try
        {
            List<Movim> movements = existantMovements.FindAll(mov => mov.AccountId == account.Id).ToList();

            List<MovimResponse> movementsToDTO = new List<MovimResponse>();
            foreach (Movim movement in movements)
            {
                movementsToDTO.Add(new MovimResponse()
                {
                    Id = movement.Id,
                    Amount = movement.Amount,
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
            Console.WriteLine("Error {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

}