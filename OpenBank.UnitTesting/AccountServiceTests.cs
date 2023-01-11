using AutoMapper;
using Moq;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Logic;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.Api.Shared;
using OpenBank.API.Application.DTO;

namespace OpenBank.UnitTesting;

[TestFixture]
public class AccountServiceTests
{
    private IAccountService _accountService;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Account _account;

    private CreateAccountRequest _accountRequest;

    public AccountServiceTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _accountService = new AccountService(_unitOfWork.Object, _mapper.Object);

        SetUp();
    }

    [SetUp]
    public void SetUp()
    {
        _account = new Account()
        {
            UserId = 1,
            Created_at = DateTime.UtcNow,
            Balance = 100,
            Currency = "EUR",
            Id = 1
        };

        _accountRequest = new CreateAccountRequest()
        {
            Amount = 100,
            Currency = "EUR",
        };
    }

    [Test]
    public void GetAccounts_ReceivesUserID_ReturnsAccounts()
    {
        int userId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetAccountsAsync(userId)).ReturnsAsync(new List<Account>());

        var result = _accountService.GetAccounts(userId);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetAccounts_ServerFails_ThrowsException()
    {
        int userId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetAccountsAsync(userId)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountService.GetAccounts(userId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.GetAccounts));
    }

    [Test]
    public async Task GetAccountById_ReceivesAccountId_ReturnsAccount()
    {
        int userId = 1;
        int accountId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.IsUserAccountAsync(accountId, userId)).ReturnsAsync(true);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).ReturnsAsync(_account);

        var result = await _accountService.GetAccountById(accountId, userId);

        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result?.Id, accountId);
        Assert.AreEqual(result?.UserId, userId);
        Assert.AreEqual(result?.Currency, _account.Currency);
    }

    [Test]
    public async Task GetAccountById_ReceivesNonExistantId_ReturnsNull()
    {
        int userId = 1;
        int accountId = 1;
        Account? accountIsNull = null;

        _unitOfWork.Setup(acc => acc.accountRepository.IsUserAccountAsync(accountId, userId)).ReturnsAsync(true);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).ReturnsAsync(accountIsNull);

        var result = await _accountService.GetAccountById(accountId, userId);

        Assert.That(result, Is.Null);
    }

    public void GetAccountById_ServerFails_ThrowsException()
    {
        int userId = 1;
        int accountId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountService.GetAccountById(accountId, userId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.FailedTransfer));
    }

    [Test]
    public async Task CreateAccount_GetsParametersToCreateAcc_ReturnsCreatedAccount()
    {
        int userId = 1;
        _account.Id = 1;
        _account.UserId = 1;

        _unitOfWork.Setup(repo => repo.accountRepository.AddAsync(_account)).ReturnsAsync((userId, _account));

        var result = await _accountService.CreateAccount(userId, _accountRequest);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateAccount_ServerFails_ThrowsException()
    {
        int userId = 1;
        _unitOfWork.Setup(repo => repo.accountRepository.AddAsync(It.IsAny<Account>())).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountService.CreateAccount(userId, _accountRequest));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.CreateAccount));
    }
}