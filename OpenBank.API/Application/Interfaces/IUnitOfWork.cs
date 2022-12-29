namespace OpenBank.API.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository userRepository { get; }
    IAccountRepository accountRepository { get; }
    ITransferRepository transferRepository { get; }
    ITokenHandler tokenHandler { get; }
    ILogger<Object> loggerHandler {get; }
}