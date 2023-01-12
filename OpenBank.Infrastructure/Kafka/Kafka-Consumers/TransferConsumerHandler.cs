using Confluent.Kafka;

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
                    System.Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
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