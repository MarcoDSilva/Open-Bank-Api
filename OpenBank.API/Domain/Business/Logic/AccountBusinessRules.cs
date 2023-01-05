using AutoMapper;
using OpenBank.Api.Shared;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Logic;

public class AccountBusinessRules : IAccountBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountBusinessRules(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new account for the user
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with the request back if succeded the creation</returns>
    /// </summary>
    public async Task<(bool, Account)> CreateAccount(int idUser, Account createAccount)
    {
        try
        {
            var result = await _unitOfWork.accountRepository.AddAsync(createAccount);

            if (result.Item1 <= 0)
                return (false, new Account());

            return (true, result.Item2);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(WarningDescriptions.CreateAccount);
        }
    }

    /// <summary>
    /// Gets the account by the selected id
    /// <exception>Exception in case something fails | Forbidden account in case the user is not the owner of the account </exception>
    /// <returns>A Task with the account if the account was found</returns>
    /// </summary>
    public async Task<Account?> GetAccountById(int accountId, int userId)
    {
        bool isUserAccount = await _unitOfWork.accountRepository.IsUserAccountAsync(accountId, userId);
        if (!isUserAccount)
            throw new ForbiddenAccountAccessException("Bearer");

        try
        {
            Account? account = await _unitOfWork.accountRepository.GetByIdAsync(accountId, userId);
            return account;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(WarningDescriptions.GetAccountById);
        }
    }

    /// <summary>
    /// Gets all the accounts for the queried user
    /// <exception>Exception in case something fails | Forbidden account in case the user is not the owner of the account </exception>
    /// <returns>List of AccountResponse</returns>
    /// </summary>
    public async Task<List<Account>> GetAccounts(int userId)
    {
        try
        {
            List<Account> accounts = await _unitOfWork.accountRepository.GetAccountsAsync(userId);
            return accounts;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(WarningDescriptions.GetAccounts);
        }
    }
}