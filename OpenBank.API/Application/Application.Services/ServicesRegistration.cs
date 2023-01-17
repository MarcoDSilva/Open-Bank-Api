using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Repository.Repositories;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Application.Services.Kafka.Interfaces;
using OpenBank.API.Application.Services.Kafka.Producers;
using OpenBank.API.Application.Services.Logic;

public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<ITransferService, TransferService>()
            .AddScoped<IDocumentService, DocumentService>()
            .AddScoped<ITransferProducer, TransferProducer>()
            .AddScoped<ITokenService, TokenService>();

        return services;
    }
}