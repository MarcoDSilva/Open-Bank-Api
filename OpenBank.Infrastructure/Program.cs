using NETCore.MailKit.Core;
using OpenBank.Infrastructure.Transfer.Kafka.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IEmailService, EmailService>();
        services.AddSingleton<IHostedService, TransferConsumerHandler>();
    })
    .Build();

await host.RunAsync();
