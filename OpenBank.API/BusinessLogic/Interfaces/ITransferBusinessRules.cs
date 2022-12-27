using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Enum;

namespace OpenBank.API.BusinessLogic.Interfaces;

public interface ITransferBusinessRules
{
    Task<(StatusCode, string)> TransferRequestAsync(TransferRequest transfer, int userId);
}