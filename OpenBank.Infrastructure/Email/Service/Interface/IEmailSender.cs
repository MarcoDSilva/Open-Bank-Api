using OpenBank.Infrastructure.Email.Model;

namespace OpenBank.Infrastructure.Email.Service.Interface;

public interface IEmailSender
{
    Task SendEmailAsync(EmailBase mailRequest);
}