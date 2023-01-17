namespace OpenBank.API.Application.Repository.Interfaces;

public interface IUnitOfWork
{
    IUserRepository userRepository { get; }
    IAccountRepository accountRepository { get; }
    ITransferRepository transferRepository { get; }
    IDocumentRepository documentRepository { get; }
    ITokenRepository tokenRepository { get; }
    ILogger<Object> loggerHandler {get; }
}