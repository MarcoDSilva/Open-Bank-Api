using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Enum;

namespace OpenBank.API.BusinessRules.Interfaces;

public interface ITransferBusinessRules
{
    Task<(StatusCode, string)> TransferRequestAsync(TransferRequest transfer, int userId);
}