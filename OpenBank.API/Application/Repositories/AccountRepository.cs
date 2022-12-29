using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;

namespace OpenBank.API.Application.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public AccountRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<int> Add(Account account)
    {
        var inserted = await _openBankApiDbContext.Accounts.AddAsync(account);
        var save = await _openBankApiDbContext.SaveChangesAsync();
        return save;
    }


    ///<param>accountId - Account identifier</param>
    ///<param>userId - User identifier</param>
    /// <summary>
    /// Gets the queried account to the logged user
    /// </summary>
    /// <returns>AccountResponse / empty AccountResponse</returns>
    public async Task<AccountResponse?> GetById(int accountId, int userId)
    {
        List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();

        Account? account = accountList.Find(acc => acc.Id == accountId && acc.UserId == userId);

        return account is null ? null : AccountToDTO(account);
    }

    public async Task<Account?> GetById(int accountId)
    {
        List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();

        Account? account = accountList.Find(acc => acc.Id == accountId);

        return account is null ? null : account;
    }

    ///<param>userId - User identifier</param>
    /// <summary>
    /// Gets all the accounts belonging to the user
    /// <returns>A Task with a list of accounts</returns>
    /// </summary>
    public async Task<List<AccountResponse>> GetAccounts(int userId)
    {
        List<Account> existantAccounts = await _openBankApiDbContext.Accounts.ToListAsync();
        List<Account> userAccounts = existantAccounts.FindAll(acc => acc.UserId == userId).ToList();

        List<AccountResponse> accountResponses = new List<AccountResponse>();

        userAccounts.ForEach(acc => accountResponses.Add(AccountToDTO(acc)));
        return accountResponses;
    }

    ///<param>accountId - Account identifier</param>
    /// <summary>
    /// Gets all the account movements belonging to the account
    /// <returns>A list with the account movements</returns>
    /// </summary>
    public async Task<List<MovimResponse>> GetMovements(int accountId)
    {
        var existantMovements = await _openBankApiDbContext.Transfers.ToListAsync();

        List<Transfer> movements = existantMovements.FindAll(mov => mov.AccountId == accountId).ToList();
        List<MovimResponse> movementsDTO = new List<MovimResponse>();

        movements.ForEach(mov => movementsDTO.Add(TransferToDTO(mov)));

        return movementsDTO;
    }

    public async Task<bool> IsUserAccount(int accountId, int userId)
    {
        List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();
        Account? account = accountList.Find(acc => acc.Id == accountId);
        return account?.UserId == userId;
    }

    public void Update(Account account)
    {
        _openBankApiDbContext.Accounts.Update(account);
    }
    
    // ============= MAPPERS =================

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
    private MovimResponse TransferToDTO(Transfer movement)
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