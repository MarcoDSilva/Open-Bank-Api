using System.Text.Json;
using Confluent.Kafka;
using OpenBank.API.Application.DTO;

namespace OpenBank.Infrastructure.Transfer.Kafka.Consumers;

public class TransferConsumerHandler : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly string? topic;

    public TransferConsumerHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        topic = _configuration["Kafka:Transfers"];

    }

    public Task StartAsync(CancellationToken cancellationToken)
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

                    System.Console.WriteLine(
                        $"Dear {communication.toUser.userName} you just received "
                        + $"the ammount of {communication.amount} {communication.currency}, on your account number {communication.toUser.accountId} "
                        + $"from the account number {communication.fromUser.accountId} that belongs to the user {communication.fromUser.userName}."
                        + $"This was sent by the publisher from {consumer.TopicPartitionOffset}");

                    // send email
                }
            }
            catch (Exception)
            {
                builder.Close();
            }
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}