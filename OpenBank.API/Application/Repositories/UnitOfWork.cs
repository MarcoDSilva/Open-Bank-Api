using OpenBank.API.Application.Interfaces;

namespace OpenBank.API.Application;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository userRepository { get; }
    public IAccountRepository accountRepository { get; }
    public ITransferRepository transferRepository { get; }
    public ITokenHandler tokenHandler { get; }
    public ILogger<Object> loggerHandler { get; }

    public UnitOfWork(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ITransferRepository transferRepository,
        ITokenHandler tokenHandler,
        ILogger<Object> loggerHandler
        )
    {
        this.userRepository = userRepository;
        this.accountRepository = accountRepository;
        this.transferRepository = transferRepository;
        this.tokenHandler = tokenHandler;
        this.loggerHandler = loggerHandler;
    }
}