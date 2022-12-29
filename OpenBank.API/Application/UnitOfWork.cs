using OpenBank.API.Application.Interfaces;

namespace OpenBank.API.Application;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository userRepository { get; }
    public IAccountRepository accountRepository { get; }
    public ITransferRepository TransferRepository { get; }
    public ITokenHandler tokenHandler { get; }
    public ILogger<Object> loggerHandler { get; }

    public UnitOfWork(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ITransferRepository TransferRepository,
        ITokenHandler tokenHandler,
        ILogger<Object> loggerHandler
        )
    {
        this.userRepository = userRepository;
        this.accountRepository = accountRepository;
        this.TransferRepository = TransferRepository;
        this.tokenHandler = tokenHandler;
        this.loggerHandler = loggerHandler;
    }
}