
using OpenBank.Infrastructure.Email.Model;
using OpenBank.Infrastructure.Email.Service.Interface;
using MimeKit;
using MailKit.Net.Smtp;

namespace OpenBank.Infrastructure.Email.Service.Logic;

public class EmailSender : IEmailSender
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
            try
            {
                await client.ConnectAsync(mailRequest.SmtpServer, mailRequest.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(mailRequest.Username, mailRequest.Password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}