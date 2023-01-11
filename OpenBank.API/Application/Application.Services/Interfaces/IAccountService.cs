using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Interfaces;

public interface IAccountService
{
    Task<AccountResponse> CreateAccount(int idUser, CreateAccountRequest createAccount);
    Task<Account?> GetAccountById(int accountId, int userId);
    Task<List<AccountResponse>> GetAccounts(int userId);
}

