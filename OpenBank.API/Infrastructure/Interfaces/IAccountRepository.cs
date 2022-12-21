using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Infrastructure.Interfaces;

public interface IAccountRepository
{
    //Task<CreateAccountRequest> CreateAccount(CreateAccountRequest createAccount);
    Task<CreateAccountRequest> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<AccountResponse> GetAccountById(int accountId, int userId);
    Task<List<AccountResponse>> GetAccounts(int userId);

    Task<AccountMovim> GetAccountMovements(AccountResponse account);
}