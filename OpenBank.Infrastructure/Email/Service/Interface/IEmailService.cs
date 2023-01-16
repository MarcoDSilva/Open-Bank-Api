using OpenBank.Infrastructure.Email.Model;

namespace OpenBank.Infrastructure.Email.Service.Interface;

public interface IEmailService
{
    Task SendEmailAsync(EmailBase mailRequest);
}