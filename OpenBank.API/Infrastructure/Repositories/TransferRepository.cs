using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.DTO;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.Enum;
using OpenBank.Api.Infrastructure.Shared;

namespace OpenBank.API.Infrastructure.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public TransferRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<(StatusCode, string)> TransferRequest(TransferRequest transfer)
    {
        var accountsList = await _openBankApiDbContext.Accounts.ToListAsync();
        Account? accountFrom = accountsList.Find(acc => acc.Id == transfer.From_account);
        Account? accountTo = accountsList.Find(acc => acc.Id == transfer.To_account);

        // validation
        var validation = ValidateAccountsForTransfer(accountFrom, accountTo, transfer);
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
            Balance = transfer.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = TypeOfMovement.Debit
        };

        Movim accountToMovement = new Movim()
        {
            Account = accountTo,
            Balance = transfer.Amount,
            Created_at = DateTime.UtcNow,
            OperationType = TypeOfMovement.Credit
        };

        try
        {
            _openBankApiDbContext.Accounts.Update(accountFrom);
            _openBankApiDbContext.Accounts.Update(accountTo);
            _openBankApiDbContext.Movim.Add(accountFromMovement);
            _openBankApiDbContext.Movim.Add(accountToMovement);

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

    private (StatusCode, string) ValidateAccountsForTransfer(Account? fromAcc, Account? toAcc, TransferRequest transfer)
    {
        if (fromAcc is null)
            return (StatusCode.NotFound, "Account_from was not found.");

        if (toAcc is null)
            return (StatusCode.NotFound, "Account_to was not found.");

        if (fromAcc.Balance < transfer.Amount)
            return (StatusCode.BadRequest, "Account_from balance is lower than the transfer amount.");

        if (!fromAcc.Currency.Equals(toAcc.Currency))
            return (StatusCode.BadRequest, "Accounts have different currencies.");

        return (StatusCode.Sucess, "");
    }
}