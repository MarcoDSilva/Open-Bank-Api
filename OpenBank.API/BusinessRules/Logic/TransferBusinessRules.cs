using OpenBank.Api.Application.Shared;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Enum;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.BusinessRules.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessRules;

public class TransferBusinessRules : ITransferBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferBusinessRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(StatusCode, string)> TransferRequestAsync(TransferRequest transfer, int userId)
    {
        Account? accountFrom = await _unitOfWork.accountRepository.GetById(transfer.From_account);
        Account? accountTo = await _unitOfWork.accountRepository.GetById(transfer.To_account);

        // validation
        (StatusCode, string) validation = ValidateAccountsForTransfer(accountFrom, accountTo, transfer, userId);
        if (validation.Item1 != StatusCode.Sucess)
        {
            return validation;
        }

        // update values
        accountFrom.Balance -= transfer.Amount;
        accountTo.Balance += transfer.Amount;

        // DTO to model for updates
        Transfer accountFromMovement = new Transfer()
        {
            Account = accountFrom,
            Amount = transfer.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = TypeOfMovement.Debit
        };

        Transfer accountToMovement = new Transfer()
        {
            Account = accountTo,
            Amount = transfer.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = TypeOfMovement.Credit
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

            return (StatusCode.ServerError, "Operation could not be concluded");
        }
        catch (Exception e)
        {
            return (StatusCode.ServerError, e.Message);
        }
    }

    private (StatusCode, string) ValidateAccountsForTransfer(Account? fromAcc, Account? toAcc, TransferRequest transfer, int userId)
    {
        if (fromAcc is null)
            return (StatusCode.NotFound, "Account_from was not found.");

        if (toAcc is null)
            return (StatusCode.NotFound, "Account_to was not found.");

        if (fromAcc.UserId != userId)
            return (StatusCode.Forbidden, "Bearer");

        if (fromAcc.Balance < transfer.Amount)
            return (StatusCode.BadRequest, "Account_from balance is lower than the transfer amount.");

        if (fromAcc.Currency != toAcc.Currency)
            return (StatusCode.BadRequest, "Accounts have different currencies.");

        return (StatusCode.Sucess, "");
    }
}