using System.Text.Json;
using Confluent.Kafka;
using OpenBank.Infrastructure.Entities.DTO;
using OpenBank.Infrastructure.Entities.Models;
using OpenBank.Infrastructure.Services.Interface;

namespace OpenBank.Infrastructure.Transfer.Kafka.Consumers;

public class TransferConsumerHandler : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailService;
    private readonly string? topic;

    private readonly string TransferSent = "Transfer Sent";
    private readonly string TransferReceived = "Transfer Received";
    private readonly string TransferSameOwner = "Transfer Between Accounts";


    public TransferConsumerHandler(IConfiguration configuration, IEmailSender emailService)
    {
        _configuration = configuration;
        topic = _configuration["Kafka:Transfers"];
        _emailService = emailService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var conf = new ConsumerConfig
        {
            GroupId = "transfer_consumer_group",
            BootstrapServers = _configuration["Kafka:Bootstrap-Server"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
        {
            builder.Subscribe(topic);
            var cancelToken = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    var communication = JsonSerializer.Deserialize<TransferCommunication>(consumer.Message.Value);

                    if (communication is null) return;

                    if (communication?.toUser.email.Equals(communication?.fromUser.email) == true)
                    {
                        await SendSameOwnerEmail(communication);
                    }
                    else
                    {
                        await SendFromEmail(communication);
                        await SendToEmail(communication);
                    }
                }
            }
            catch (Exception)
            {
                builder.Close();
            }
        }
        return;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }


    private async Task SendSameOwnerEmail(TransferCommunication communication)
    {
        var message = CreateSameOwnerMessage(communication);
        Console.WriteLine(message);

        var email = CreateEmail(communication?.toUser.email, TransferSameOwner, message);
        await _emailService.SendEmailAsync(email);
    }

    private async Task SendFromEmail(TransferCommunication communication)
    {
        var fromMessage = CreateFromMessage(communication);
        Console.WriteLine(fromMessage);

        var fromEmail = CreateEmail(communication?.fromUser.email, TransferSent, fromMessage);
        await _emailService.SendEmailAsync(fromEmail);
    }

    private async Task SendToEmail(TransferCommunication communication)
    {
        var toMessage = CreateToMessage(communication);
        Console.WriteLine(toMessage);

        var toEmail = CreateEmail(communication?.toUser.email, TransferReceived, toMessage);
        await _emailService.SendEmailAsync(toEmail);
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