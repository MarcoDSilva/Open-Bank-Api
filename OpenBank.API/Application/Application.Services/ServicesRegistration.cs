using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Application.Services.Logic;

public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IUserBusinessRules, UserBusinessRules>()
            .AddScoped<IAccountBusinessRules, AccountBusinessRules>()
            .AddScoped<ITransferBusinessRules, TransferBusinessRules>()
            .AddScoped<IDocumentBusinessRules, DocumentBusinessRules>();

        return services;
    }
}