using AutoMapper;
using OpenBank.Api.Shared;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Logic;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;


    public TransferService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<string> TransferRequestAsync(TransferRequest request, int userId)
    {
        Account? accountFrom = await _unitOfWork.accountRepository.GetByIdAsync(request.From_account);
        Account? accountTo = await _unitOfWork.accountRepository.GetByIdAsync(request.To_account);

        if (accountFrom is null || accountTo is null) throw new MovementAccountNotFoundException(AccountDescriptions.AccountNotFound);
        if (accountFrom.UserId != userId) throw new ForbiddenAccountAccessException(AccountDescriptions.BearerNotAllowed);
        if (accountTo.Balance < request.Amount) throw new LowerBalanceException(AccountDescriptions.LowerBalance);
        if (accountTo.Currency != accountFrom.Currency) throw new DifferentCurrenciesException(AccountDescriptions.DifferentCurrencies);

        // update values
        accountFrom.Balance -= request.Amount;
        accountTo.Balance += request.Amount;

        // DTO to model for updates
        Transfer accountFromMovement = new Transfer()
        {
            Account = accountFrom,
            Amount = request.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = AccountDescriptions.Debit
        };

        Transfer accountToMovement = new Transfer()
        {
            Account = accountTo,
            Amount = request.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = AccountDescriptions.Credit
        };

        try
        {
            _unitOfWork.accountRepository.Update(accountFrom);
            _unitOfWork.accountRepository.Update(accountTo);
            var accountFromResult = await _unitOfWork.transferRepository.AddAsync(accountFromMovement);
            var accountToResult = await _unitOfWork.transferRepository.AddAsync(accountToMovement);

            bool isSaved = await _unitOfWork.transferRepository.SaveAsync();

            return isSaved ? "Transfer was completed with success" : WarningDescriptions.FailedTransfer;
        }
        catch (Exception e)
        {
            throw new Exception(WarningDescriptions.FailedTransfer);
        }
    }

    public async Task<List<MovementResponse>> GetAccountMovementsAsync(int accountId)
    {
        try
        {
            var movements = await _unitOfWork.accountRepository.GetAccountMovementsAsync(accountId);

            List<MovementResponse> movementsDTO = new List<MovementResponse>();
            movements.ForEach(mov => movementsDTO.Add(_mapper.Map<Transfer, MovementResponse>(mov)));

            return movementsDTO;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }
}