using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;

public interface IAccountRepository
{
    Task<int> AddAsync(Account account);
    void Update(Account account);

    Task<Account?> GetById(int accountId);
    Task<List<Account>> GetAccounts(int userId);
    Task<bool> IsUserAccount(int accountId, int userId);
    Task<List<Transfer>> GetMovements(int accountId);
    Task<Account?> GetById(int accountId, int userId);

}