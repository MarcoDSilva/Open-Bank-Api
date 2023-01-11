using OpenBank.API.Application.DTO;
using OpenBank.API.Enum;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Interfaces;

public interface ITransferBusinessRules
{
    Task<string> TransferRequestAsync(Movement transfer, int userId);
    Task<List<Transfer>> GetAccountMovementsAsync(int accountId);
}