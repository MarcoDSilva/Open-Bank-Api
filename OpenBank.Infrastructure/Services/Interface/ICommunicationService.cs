using OpenBank.Infrastructure.Entities.DTO;

namespace OpenBank.Infrastructure.Services.Interface;
public interface ICommunicationService
{
    Task SendEmailAsync(TransferCommunication communication);
}