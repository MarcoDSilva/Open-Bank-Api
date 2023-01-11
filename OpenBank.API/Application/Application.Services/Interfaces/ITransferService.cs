using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Interfaces;

public interface ITransferService
{
    Task<string> TransferRequestAsync(Movement transfer, int userId);
    Task<List<Transfer>> GetAccountMovementsAsync(int accountId);
}