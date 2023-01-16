using System.Text.Json;
using Confluent.Kafka;
using OpenBank.Infrastructure.Entities.DTO;
using OpenBank.Infrastructure.Services.Interface;

namespace OpenBank.Infrastructure.Transfer.Kafka.Consumers;

public class TransferConsumerHandler : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ICommunicationService _communicationService;
    private readonly string? topic;

    public TransferConsumerHandler(IConfiguration configuration, ICommunicationService communicationService)
    {
        _configuration = configuration;
        topic = _configuration["Kafka:Transfers"];
        _communicationService = communicationService;
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

                    await _communicationService.SendEmailAsync(communication);
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
}