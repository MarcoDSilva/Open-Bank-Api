using OpenBank.Api.Data;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Enum;
using OpenBank.API.BusinessLogic.Interfaces;

namespace OpenBank.API.BusinessLogic;

public class TransferBusinessRules : ITransferBusinessRules
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public TransferBusinessRules(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public Task<(StatusCode, string)> TransferRequestAsync(TransferRequest transfer, int userId)
    {
        throw new NotImplementedException();
    }
}