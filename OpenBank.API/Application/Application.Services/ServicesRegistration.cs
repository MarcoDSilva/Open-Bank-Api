using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Application.Services.Logic;

public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<ITransferService, TransferService>()
            .AddScoped<IDocumentService, DocumentService>();

        return services;
    }
}