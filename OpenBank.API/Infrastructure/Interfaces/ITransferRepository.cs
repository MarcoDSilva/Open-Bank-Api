using OpenBank.API.DTO;
namespace OpenBank.API.Infrastructure.Interfaces;

public interface ITransferRepository 
{
    Task<TransferRequest> TransferRequest(TransferRequest transfer);
}