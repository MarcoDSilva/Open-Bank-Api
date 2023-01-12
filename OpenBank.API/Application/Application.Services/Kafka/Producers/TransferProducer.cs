using Confluent.Kafka;
using OpenBank.API.Application.Services.Kafka.Interfaces;

namespace OpenBank.API.Application.Services.Kafka.Producers;

public class TransferProducer : ITransferProducer
{
    readonly IConfiguration _configuration;

    public readonly ProducerConfig producerConfig;
    public readonly string? topic;

    public TransferProducer(IConfiguration configuration)
    {
        _configuration = configuration;

        producerConfig = new ProducerConfig()
        {
            BootstrapServers = _configuration["Kafka:Bootstrap-Server"],
        };

        topic = _configuration["Kafka:Transfers"];
    }

    public object SendToKafka(string message)
    {
        using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
        {
            try
            {
                return producer
                    .ProduceAsync(topic, new Message<Null, string> { Value = message })
                    .GetAwaiter()
                    .GetResult();

            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Message could not be produced. Error: ${e.Message}");
            }
        }
        return null;
    }
}