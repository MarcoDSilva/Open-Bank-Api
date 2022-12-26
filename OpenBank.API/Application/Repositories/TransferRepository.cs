using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.Enum;
using OpenBank.Api.Application.Shared;

namespace OpenBank.API.Application.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public TransferRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<(StatusCode, string)> TransferRequestAsync(TransferRequest transfer, int userId)
    {
        var accountsList = await _openBankApiDbContext.Accounts.ToListAsync();
        Account? accountFrom = accountsList.Find(acc => acc.Id == transfer.From_account);
        Account? accountTo = accountsList.Find(acc => acc.Id == transfer.To_account);

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
        Movim accountFromMovement = new Movim()
        {
            Account = accountFrom,
            Amount = transfer.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = TypeOfMovement.Debit
        };

        Movim accountToMovement = new Movim()
        {
            Account = accountTo,
            Amount = transfer.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = TypeOfMovement.Credit
        };

        try
        {
            _openBankApiDbContext.Accounts.Update(accountFrom);
            _openBankApiDbContext.Accounts.Update(accountTo);
            await _openBankApiDbContext.Movim.AddAsync(accountFromMovement);
            await _openBankApiDbContext.Movim.AddAsync(accountToMovement);

            var result = await _openBankApiDbContext.SaveChangesAsync();

            if (result > 0)
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