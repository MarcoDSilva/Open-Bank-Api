namespace OpenBank.API.Application.Services.Kafka.Interfaces;

public interface ITransferProducer
{
    object Publish(string message);
}