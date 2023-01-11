using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repository.Interfaces;

public interface ITransferRepository 
{
    Task<Transfer> AddAsync(Transfer transfer);
    Task<bool> SaveAsync();
}