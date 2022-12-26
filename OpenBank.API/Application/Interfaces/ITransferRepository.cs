using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Enum;

namespace OpenBank.API.Application.Interfaces;

public interface ITransferRepository 
{
    Task<(StatusCode,string)> TransferRequestAsync(TransferRequest transfer, int userId);
}