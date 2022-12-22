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
            Currency = createAccount.Currency,
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
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception("Error while creating the user");
        }
    }

    /// <summary>
    /// Gets the account by the selected id
    /// <exception>Exception in case something fails | Forbidden account in case the user is not the owner of the account </exception>
    /// <returns>A Task with the account if the account was found</returns>
    /// </summary>
    public async Task<AccountResponse> GetAccountById(int accountId, int userId)
    {
        try
        {
            List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();

            Account? account = accountList.Find(acc => acc.Id == accountId);

            if (account?.UserId != userId)
                throw new ForbiddenAccountAccessException("Bearer"); // the error has to be issued as bearer for JWT to allow the forbid

            AccountResponse accountDTO = AccountToDTO(account);

            return accountDTO;
        }
        catch (ForbiddenAccountAccessException e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new ForbiddenAccountAccessException("Bearer");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }


    /// <summary>
    /// Gets all the accounts belonging to the user
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with a list of accounts</returns>
    /// </summary>
    public async Task<List<AccountResponse>> GetAccounts(int userId)
    {
        List<Account> existantAccounts = await _openBankApiDbContext.Accounts.ToListAsync();

        try
        {
            List<Account> userAccounts = existantAccounts.FindAll(acc => acc.UserId == userId).ToList();

            List<AccountResponse> accountResponses = new List<AccountResponse>();

            userAccounts.ForEach(acc => accountResponses.Add(AccountToDTO(acc)));

            return accountResponses;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception("Error while obtaining the accounts");
        }
    }

    /// <summary>
    /// Gets all the account movements belonging to the account
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with the account movements</returns>
    /// </summary>
    public async Task<AccountMovim> GetAccountMovements(AccountResponse account)
    {
        var existantMovements = await _openBankApiDbContext.Movim.ToListAsync();

        try
        {
            List<Movim> movements = existantMovements.FindAll(mov => mov.AccountId == account.Id).ToList();

            List<MovimResponse> movementsDTO = new List<MovimResponse>();

            movements.ForEach(mov => movementsDTO.Add(MovimToDTO(mov)));

            AccountMovim accountMovim = new AccountMovim()
            {
                Account = account,
                Movimentos = movementsDTO,
            };

            return accountMovim;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Turns the account model into a DTO to return to the requester
    /// </summary>
    private AccountResponse AccountToDTO(Account account)
    {
        return new AccountResponse()
        {
            Balance = account.Balance,
            Created_at = account.Created_at,
            Currency = account.Currency,
            Id = account.Id
        };
    }

    /// <summary>
    /// Turns the movement model into a DTO to return to the requester
    /// </summary>
    private MovimResponse MovimToDTO(Movim movement)
    {
        return new MovimResponse()
        {
            Id = movement.Id,
            Amount = movement.Amount,
            Created_at = movement.Created_at,
            OperationType = movement.OperationType
        };
    }
}