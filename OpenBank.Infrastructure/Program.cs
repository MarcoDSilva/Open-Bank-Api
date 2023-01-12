using OpenBank.Infrastructure.Transfer.Kafka.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IHostedService, TransferConsumerHandler>();
    })
    .Build();

await host.RunAsync();
