
using OpenBank.Infrastructure.Email.Model;
using OpenBank.Infrastructure.Email.Service.Interface;
using MimeKit;
using MailKit.Net.Smtp;

namespace OpenBank.Infrastructure.Email.Service.Logic;

public class MailService : IEmailService
{
    public async Task SendEmailAsync(EmailBase mailRequest)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(string.Empty, mailRequest.From));
        email.To.Add(new MailboxAddress(string.Empty, mailRequest.To));
        email.Subject = mailRequest.Subject;
        email.Body = new TextPart("plain") { Text = mailRequest.Body };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(mailRequest.SmtpServer, mailRequest.Port, false);
            await client.AuthenticateAsync(mailRequest.Username, mailRequest.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
    }
}