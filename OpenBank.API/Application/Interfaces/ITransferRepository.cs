using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.Interfaces;

public interface ITransferRepository 
{
    Task<Transfer> AddAsync(Transfer transfer);
    Task<bool> SaveAsync();
}