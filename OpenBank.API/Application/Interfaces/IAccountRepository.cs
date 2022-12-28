using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;

public interface IAccountRepository
{
    Task<List<AccountResponse>> GetAccounts(int userId);
    Task<int> Add(Account account);
    Task<AccountResponse?> GetById(int accountId, int userId);
    Task<bool> IsUserAccount(int accountId, int userId);
    Task<List<MovimResponse>> GetMovements(int accountId);
}