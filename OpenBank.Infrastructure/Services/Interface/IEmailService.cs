using OpenBank.Infrastructure.Entities.Models;

namespace OpenBank.Infrastructure.Services.Interface;

public interface IEmailService
{
    Task SendEmailAsync(EmailBase mailRequest);
}