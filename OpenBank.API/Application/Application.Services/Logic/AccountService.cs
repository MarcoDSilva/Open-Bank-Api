using AutoMapper;
using OpenBank.Api.Shared;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Logic;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new account for the user
    /// <exception>Exception in case something fails </exception>
    /// <returns>A Task with the request back if succeded the creation</returns>
    /// </summary>
    public async Task<AccountResponse> CreateAccount(int idUser, CreateAccountRequest createAccount)
    {
        try
        {
            Account account = new Account()
            {
                Balance = createAccount.Amount,
                Created_at = DateTime.UtcNow,
                Currency = createAccount.Currency,
                UserId = idUser
            };

            var result = await _unitOfWork.accountRepository.AddAsync(account);

            return result.Item1 <= 0 ? new AccountResponse() : _mapper.Map<Account, AccountResponse>(result.Item2);
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
        var isUserAccount = await _unitOfWork.accountRepository.IsUserAccountAsync(accountId, userId);
        if (!isUserAccount)
            throw new ForbiddenAccountAccessException("Bearer");

        try
        {
            var account = await _unitOfWork.accountRepository.GetByIdAsync(accountId, userId);
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
    public async Task<List<AccountResponse>> GetAccounts(int userId)
    {
        try
        {
            var accounts = await _unitOfWork.accountRepository.GetAccountsAsync(userId);

            var accountResponseDTO = new List<AccountResponse>();
            accounts.ForEach(acc => accountResponseDTO.Add(_mapper.Map<Account, AccountResponse>(acc)));

            return accountResponseDTO;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(WarningDescriptions.GetAccounts);
        }
    }
}