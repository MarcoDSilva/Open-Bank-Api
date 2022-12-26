using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;

public interface IAccountRepository
{
    Task<CreateAccountRequest> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<AccountResponse> GetAccountById(int accountId, int userId);
    Task<List<AccountResponse>> GetAccounts(int userId);
    Task<AccountMovim> GetAccountMovements(AccountResponse account);
}