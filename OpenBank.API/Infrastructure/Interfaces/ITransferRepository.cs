using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Infrastructure.Enum;

namespace OpenBank.API.Infrastructure.Interfaces;

public interface ITransferRepository 
{
    Task<(StatusCode,string)> TransferRequestAsync(TransferRequest transfer, int userId);
}