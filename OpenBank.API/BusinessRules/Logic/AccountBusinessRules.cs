using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.BusinessRules.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessRules;

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

            return newAccountId <= 0 ? (false, createAccount) : (true, createAccount);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception("Error while creating the user");
        }
    }

    /// <summary>
    /// Gets the account by the selected id
    /// <exception>Exception in case something fails | Forbidden account in case the user is not the owner of the account </exception>
    /// <returns>A Task with the account if the account was found</returns>
    /// </summary>
    public async Task<AccountResponse?> GetAccountById(int accountId, int userId)
    {
        bool isUserAccount = await _unitOfWork.accountRepository.IsUserAccount(accountId, userId);
        if (!isUserAccount)
            throw new ForbiddenAccountAccessException("Bearer");

        try
        {
            AccountResponse? account = await _unitOfWork.accountRepository.GetById(accountId, userId);
            return account;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Gets all the accounts for the queried user
    /// <exception>Exception in case something fails | Forbidden account in case the user is not the owner of the account </exception>
    /// <returns>List of AccountResponse</returns>
    /// </summary>
    public async Task<List<AccountResponse>> GetAccounts(int userId)
    {
        try
        {
            List<AccountResponse> accounts = await _unitOfWork.accountRepository.GetAccounts(userId);
            return accounts;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    public async Task<List<MovimResponse>> GetAccountMovements(int accountId)
    {
        try
        {
            List<MovimResponse> movements = await _unitOfWork.accountRepository.GetMovements(accountId);

            return movements;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }
}