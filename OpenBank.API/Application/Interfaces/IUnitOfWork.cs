namespace OpenBank.API.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository userRepository { get; }
    IAccountRepository accountRepository { get; }
    ITransferRepository TransferRepository { get; }
    ITokenHandler tokenHandler { get; }
    ILogger<Object> loggerHandler {get; }
}