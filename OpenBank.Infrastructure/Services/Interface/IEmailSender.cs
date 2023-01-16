using OpenBank.Infrastructure.Entities.Models;

namespace OpenBank.Infrastructure.Services.Interface;

public interface IEmailSender
{
    Task SendEmailAsync(EmailBase mailRequest);
}