using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Repository.Repositories;

public static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<ITransferRepository, TransferRepository>()
            .AddScoped<IDocumentRepository, DocumentRepository>()
            .AddScoped<ITokenHandler, TokenHandler>();

        return services;
    }
}