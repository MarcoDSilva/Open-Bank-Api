using OpenBank.API.Application.DTO;
namespace OpenBank.API.BusinessLogic.Interfaces;

public interface IAccountBusinessRules
{
    Task<(bool, CreateAccountRequest)> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<AccountResponse> GetAccountById(int accountId, int userId);
    Task<List<AccountResponse>> GetAccounts(int userId);
    Task<AccountMovim> GetAccountMovements(AccountResponse account);
}
