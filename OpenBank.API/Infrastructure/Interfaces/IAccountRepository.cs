using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Infrastructure.Interfaces;

public interface IAccountRepository
{
    //Task<CreateAccountRequest> CreateAccount(CreateAccountRequest createAccount);
    Task<CreateAccountRequest> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<Account> GetAccountById(int accountId);
    Task<List<Account>> GetAccounts(int userId);

    Task<AccountMovim> GetAccountMovements(Account account);
}