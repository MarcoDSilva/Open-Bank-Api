using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.DTO;
using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Infrastructure.Repositories;

public class TransferRepository : ITransferRepository 
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public TransferRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<TransferRequest> TransferRequest(TransferRequest transfer)
    {
        var accountsList = await _openBankApiDbContext.Accounts.ToListAsync();
        var accountFrom = accountsList.Find(acc => acc.Id == transfer.From_account); 
        var accountTo = accountsList.Find(acc => acc.Id == transfer.To_account); 

        // fazer exception para estes casos
        if (accountFrom is null)
            throw new Exception("Account from not found");
        
        if (accountTo is null)
            throw new Exception("Account to not found");

        if (accountFrom.Balance < transfer.Amount)
            throw new Exception("AccountFrom balance is lower than the transfer amount.");            
        

        throw new NotImplementedException();
    }
}