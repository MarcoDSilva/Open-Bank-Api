using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Interfaces;

public interface ITransferService
{
    Task<string> TransferRequestAsync(TransferRequest transfer, int userId);
    Task<List<MovementResponse>> GetAccountMovementsAsync(int accountId);
}