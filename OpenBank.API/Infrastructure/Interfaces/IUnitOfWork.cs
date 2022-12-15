namespace OpenBank.API.Infrastructure.Interfaces;

public interface IUnitOfWork
{
    IUserRepository userRepository { get; }
    IAccountRepository accountRepository { get; }
    //ITransferRepository transferRepository { get; }
}