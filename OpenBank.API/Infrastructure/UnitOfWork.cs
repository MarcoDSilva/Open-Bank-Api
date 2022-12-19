using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository userRepository { get; }
    public IAccountRepository accountRepository { get; }
    public ITransferRepository transferRepository { get; }

    public UnitOfWork(IUserRepository userRepository, IAccountRepository accountRepository, ITransferRepository transferRepository)
    {
        this.userRepository = userRepository;
        this.accountRepository = accountRepository;
        this.transferRepository = transferRepository;
    }

}