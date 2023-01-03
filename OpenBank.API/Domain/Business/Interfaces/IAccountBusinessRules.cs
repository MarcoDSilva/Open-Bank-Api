using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Interfaces;

public interface IAccountBusinessRules
{
    Task<(bool, Account)> CreateAccount(int idUser, Account createAccount);
    Task<Account?> GetAccountById(int accountId, int userId);
    Task<List<Account>> GetAccounts(int userId);
}

