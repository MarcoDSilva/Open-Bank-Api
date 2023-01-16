using OpenBank.Infrastructure.Services.Interface;
using OpenBank.Infrastructure.Services.Logic;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddKafkaComponents();
    })
    .Build();

await host.RunAsync();
