using OpenBank.Api.Data;
using OpenBank.API.Domain.Entities;
using OpenBank.API.Application.Interfaces;

namespace OpenBank.API.Application.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public TransferRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<Transfer> AddAsync(Transfer transfer)
    {
        var result = await _openBankApiDbContext.Transfers.AddAsync(transfer);
        return result.Entity;
    }

    public async Task<bool> SaveAsync()
    {
        var result = await _openBankApiDbContext.SaveChangesAsync();
        return result > 0;
    }
}