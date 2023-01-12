using OpenBank.Infrastructure.Transfer.Kafka.Consumers;

public static class KafkaRegistration
{
    public static IServiceCollection AddKafkaComponents(this IServiceCollection services)
    {
        services
            .AddSingleton<IHostedService, TransferConsumerHandler>();

        return services;
    }
}