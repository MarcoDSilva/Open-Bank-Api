using OpenBank.API.Application.Repository.Interfaces;

namespace OpenBank.API.Application.Repository.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository userRepository { get; }
    public IAccountRepository accountRepository { get; }
    public ITransferRepository transferRepository { get; }
    public IDocumentRepository documentRepository { get; }

    public ITokenService tokenHandler { get; }
    public ILogger<Object> loggerHandler { get; }

    public UnitOfWork(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ITransferRepository transferRepository,
        IDocumentRepository documentRepository,
        ITokenService tokenHandler,
        ILogger<Object> loggerHandler
        )
    {
        this.userRepository = userRepository;
        this.accountRepository = accountRepository;
        this.transferRepository = transferRepository;
        this.tokenHandler = tokenHandler;
        this.loggerHandler = loggerHandler;
        this.documentRepository = documentRepository;
    }
}