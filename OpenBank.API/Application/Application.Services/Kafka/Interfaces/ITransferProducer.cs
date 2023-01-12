namespace OpenBank.API.Application.Services.Kafka.Interfaces;

public interface ITransferProducer
{
    object SendToKafka(string message);
}