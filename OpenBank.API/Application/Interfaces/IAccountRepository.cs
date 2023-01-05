using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Interfaces;

public interface IAccountRepository
{
    Task<(int, Account)> AddAsync(Account account);
    void Update(Account account);

    Task<Account?> GetByIdAsync(int accountId);
    Task<Account?> GetByIdAsync(int accountId, int userId);
    Task<List<Account>> GetAccountsAsync(int userId);
    Task<bool> IsUserAccountAsync(int accountId, int userId);
    Task<List<Transfer>> GetAccountMovementsAsync(int accountId);
}