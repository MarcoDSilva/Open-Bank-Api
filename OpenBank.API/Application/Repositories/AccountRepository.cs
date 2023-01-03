using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.API.Application.Interfaces;

namespace OpenBank.API.Application.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public AccountRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }


    /// <summary>
    /// Inserts a new account
    /// </summary>
    /// <returns>Insertion ID && account entity</returns>
    public async Task<(int, Account)> AddAsync(Account account)
    {
        var inserted = await _openBankApiDbContext.Accounts.AddAsync(account);
        var save = await _openBankApiDbContext.SaveChangesAsync();
        return (save, inserted.Entity);
    }


    ///<param>accountId - Account identifier</param>
    ///<param>userId - User identifier</param>
    /// <summary>
    /// Gets the queried account to the logged user
    /// </summary>
    /// <returns>AccountResponse / empty AccountResponse</returns>
    public async Task<Account?> GetByIdAsync(int accountId, int userId)
    {
        List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();
        Account? account = accountList.Find(acc => acc.Id == accountId && acc.UserId == userId);

        return account is null ? null : account;
    }

    public async Task<Account?> GetByIdAsync(int accountId)
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
    public async Task<List<Account>> GetAccountsAsync(int userId)
    {
        List<Account> existantAccounts = await _openBankApiDbContext.Accounts.ToListAsync();
        List<Account> userAccounts = existantAccounts.FindAll(acc => acc.UserId == userId).ToList();

        return userAccounts;
    }

    ///<param>accountId - Account identifier</param>
    /// <summary>
    /// Gets all the account movements belonging to the account
    /// <returns>A list with the account movements</returns>
    /// </summary>
    public async Task<List<Transfer>> GetAccountMovementsAsync(int accountId)
    {
        var existantMovements = await _openBankApiDbContext.Transfers.ToListAsync();
        List<Transfer> movements = existantMovements.FindAll(mov => mov.AccountId == accountId).ToList();

        return movements;
    }

    public async Task<bool> IsUserAccountAsync(int accountId, int userId)
    {
        List<Account> accountList = await _openBankApiDbContext.Accounts.ToListAsync();
        Account? account = accountList.Find(acc => acc.Id == accountId);

        return account?.UserId == userId;
    }

    public void Update(Account account)
    {
        _openBankApiDbContext.Accounts.Update(account);
    }
}