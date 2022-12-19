using OpenBank.API.DTO;
using OpenBank.API.Infrastructure.Enum;

namespace OpenBank.API.Infrastructure.Interfaces;

public interface ITransferRepository 
{
    Task<(StatusCode,string)> TransferRequestAsync(TransferRequest transfer);
}