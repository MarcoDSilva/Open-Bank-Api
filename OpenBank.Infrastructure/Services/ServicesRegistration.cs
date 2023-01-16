using OpenBank.Infrastructure.Services.Interface;
using OpenBank.Infrastructure.Services.Logic;

public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddTransient<ICommunicationService, CommunicationService>()
            .AddSingleton<IEmailService, EmailService>();

        return services;
    }
}