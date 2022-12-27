using OpenBank.Api.Data;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.BusinessLogic.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessLogic;

public class AccountBusinessRules : IAccountBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountBusinessRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new account for the user
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with the request back if succeded the creation</returns>
    /// </summary>
    public async Task<(bool, CreateAccountRequest)> CreateAccount(int idUser, CreateAccountRequest createAccount)
    {
        Account account = new Account()
        {
            Balance = createAccount.Amount,
            Created_at = DateTime.UtcNow,
            Currency = createAccount.Currency,
            UserId = idUser
        };

        try
        {
            var newAccountId = await _unitOfWork.accountRepository.Add(account);

            if (newAccountId <= 0) return (false, createAccount);
            return (true, createAccount);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception("Error while creating the user");
        }
    }

    public Task<AccountResponse> GetAccountById(int accountId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<AccountMovim> GetAccountMovements(AccountResponse account)
    {
        throw new NotImplementedException();
    }

    public Task<List<AccountResponse>> GetAccounts(int userId)
    {
        throw new NotImplementedException();
    }    
}