using OpenBank.Infrastructure.Entities.DTO;
using OpenBank.Infrastructure.Entities.Models;
using OpenBank.Infrastructure.Services.Interface;

namespace OpenBank.Infrastructure.Services.Logic;

public class CommunicationService : ICommunicationService
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    private readonly string TransferSent = "Transfer Sent";
    private readonly string TransferReceived = "Transfer Received";
    private readonly string TransferSameOwner = "Transfer Between Accounts";

    public CommunicationService(IConfiguration configuration, IEmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task SendEmailAsync(TransferCommunication communication)
    {
        if (communication is null) return;

        if (communication?.toUser.Id == communication?.fromUser.Id)
        {
            await SendSameOwnerEmail(communication);
        }
        else
        {
            await SendEmail(communication, TransferSent);
            await SendEmail(communication, TransferReceived);
        }
        return;
    }

    private async Task SendSameOwnerEmail(TransferCommunication communication)
    {
        var message = CreateSameOwnerMessage(communication);
        Console.WriteLine(message);

        var email = CreateEmail(communication?.toUser.email, TransferSameOwner, message);
        await _emailService.SendEmailAsync(email);
    }

    private async Task SendEmail(TransferCommunication communication, string subject)
    {
        var message = subject.Equals(TransferSent) ? CreateFromMessage(communication) : CreateToMessage(communication);
        Console.WriteLine(message);

        var userEmail = subject.Equals(TransferSent) ? communication?.fromUser.email : communication?.toUser.email;

        var email = CreateEmail(userEmail, subject, message);
        await _emailService.SendEmailAsync(email);
    }

    private EmailBase CreateEmail(string recipient, string subject, string message)
    {
        return new EmailBase()
        {
            From = _configuration["EmailConfiguration:From"],
            To = recipient,
            Subject = subject,
            Username = _configuration["EmailConfiguration:Username"],
            Password = _configuration["EmailConfiguration:Password"],
            Port = int.Parse(_configuration["EmailConfiguration:Port"]),
            SmtpServer = _configuration["EmailConfiguration:SmtpServer"],
            Body = message
        };
    }

    private string CreateFromMessage(TransferCommunication communication)
    {
        return string.Concat("Dear", communication?.fromUser.userName,
                " your transfer to the account with the number ", communication?.toUser.accountId,
                " was completed with success.\n", "The ammount of ", communication?.amount, " ", communication?.currency,
                " was deducted from your account number ", communication?.fromUser.accountId);
    }

    private string CreateToMessage(TransferCommunication communication)
    {
        return string.Concat("Dear", communication?.toUser.userName,
               " you received on your account with the number ", communication?.toUser.accountId,
               " the ammount of ", communication?.amount, " ", communication?.currency,
               " That was sent from the account belonging to ", communication?.fromUser.userName);
    }

    private string CreateSameOwnerMessage(TransferCommunication communication)
    {
        return string.Concat("Dear", communication?.fromUser.userName,
                " your transfer to your account number ", communication?.fromUser.accountId,
                " was completed with success.\n", "The ammount of ", communication?.amount, communication?.currency, " was sent.");
    }
}