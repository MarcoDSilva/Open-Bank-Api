using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessRules.Interfaces;

public interface IAccountBusinessRules
{
    Task<(bool, AccountResponse)> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<AccountResponse?> GetAccountById(int accountId, int userId);
    Task<List<AccountResponse>> GetAccounts(int userId);
    Task<List<MovementResponse>> GetAccountMovements(int accountId);
}

