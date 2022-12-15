using OpenBank.API.Models.Entities;

namespace OpenBank.API.Data;

public interface IAccountRepository
{
    Task<CreateAccountRequest> CreateAccount(CreateAccountRequest createAccount);
    Task<CreateAccountRequest> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<Account> GetAccountById(int accountId);
    Task<List<Account>> GetAccounts(int userId);
}