IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddServices();
        services.AddKafkaComponents();
    })
    .Build();

await host.RunAsync();
