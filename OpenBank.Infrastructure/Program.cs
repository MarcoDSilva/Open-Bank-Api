using OpenBank.Infrastructure.Email.Service.Interface;
using OpenBank.Infrastructure.Email.Service.Logic;
using OpenBank.Infrastructure.Transfer.Kafka.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddSingleton<IHostedService, TransferConsumerHandler>();
    })
    .Build();

await host.RunAsync();
