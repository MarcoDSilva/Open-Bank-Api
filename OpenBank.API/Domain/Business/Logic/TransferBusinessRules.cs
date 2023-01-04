using OpenBank.Api.Shared;
using OpenBank.API.Enum;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Logic;

public class TransferBusinessRules : ITransferBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferBusinessRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(StatusCode, string)> TransferRequestAsync(Movement movement, int userId)
    {
        Account? accountFrom = await _unitOfWork.accountRepository.GetByIdAsync(movement.accountFrom);
        Account? accountTo = await _unitOfWork.accountRepository.GetByIdAsync(movement.accountTo);

        // validation
        (StatusCode, string) validation = ValidateAccountsForTransfer(accountFrom, accountTo, movement, userId);
        if (validation.Item1 != StatusCode.Sucess)
        {
            return validation;
        }

        // update values
        accountFrom.Balance -= movement.Amount;
        accountTo.Balance += movement.Amount;

        // DTO to model for updates
        Transfer accountFromMovement = new Transfer()
        {
            Account = accountFrom,
            Amount = movement.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = AccountDescriptions.Debit
        };

        Transfer accountToMovement = new Transfer()
        {
            Account = accountTo,
            Amount = movement.Amount,
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

            if (isSaved)
                return (StatusCode.Sucess, "Transfer was completed with success");

            return (StatusCode.ServerError, ErrorDescriptions.FailedTransfer);
        }
        catch (Exception e)
        {
            throw new Exception(ErrorDescriptions.FailedTransfer);
        }
    }

    public async Task<List<Transfer>> GetAccountMovementsAsync(int accountId)
    {
        try
        {
            List<Transfer> movements = await _unitOfWork.accountRepository.GetAccountMovementsAsync(accountId);
            return movements;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    private (StatusCode, string) ValidateAccountsForTransfer(Account? fromAcc, Account? toAcc, Movement transfer, int userId)
    {
        if (fromAcc is null)
            return (StatusCode.NotFound, AccountDescriptions.AccountNotFound);

        if (toAcc is null)
            return (StatusCode.NotFound, AccountDescriptions.AccountNotFound);

        if (fromAcc.UserId != userId)
            return (StatusCode.Forbidden, AccountDescriptions.BearerNotAllowed);

        if (fromAcc.Balance < transfer.Amount)
            return (StatusCode.BadRequest, AccountDescriptions.LowerBalance);

        if (fromAcc.Currency != toAcc.Currency)
            return (StatusCode.BadRequest, AccountDescriptions.DifferentCurrencies);

        return (StatusCode.Sucess, "");
    }
}