using AutoMapper;
using Moq;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Logic;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.Api.Shared;

namespace OpenBank.UnitTesting;

[TestFixture]
public class AccountBusinessRulesTests
{
    private IAccountBusinessRules _accountBusiness;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Account _account;

    public AccountBusinessRulesTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _accountBusiness = new AccountBusinessRules(_unitOfWork.Object, _mapper.Object);

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
    }

    [Test]
    public void GetAccounts_ReceivesUserID_ReturnsAccounts()
    {
        int userId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetAccountsAsync(userId)).ReturnsAsync(new List<Account>());

        var result = _accountBusiness.GetAccounts(userId);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetAccounts_ServerFails_ThrowsException()
    {
        int userId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetAccountsAsync(userId)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountBusiness.GetAccounts(userId));

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

        var result = await _accountBusiness.GetAccountById(accountId, userId);

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

        var result = await _accountBusiness.GetAccountById(accountId, userId);

        Assert.That(result, Is.Null);
    }

    public void GetAccountById_ServerFails_ThrowsException()
    {
        int userId = 1;
        int accountId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountBusiness.GetAccountById(accountId, userId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.FailedTransfer));
    }

    [Test]
    public async Task CreateAccount_GetsParametersToCreateAcc_ReturnsCreatedAccount()
    {
        int userId = 1;
        _unitOfWork.Setup(repo => repo.accountRepository.AddAsync(It.IsAny<Account>())).ReturnsAsync((userId, new Account()));

        var result = await _accountBusiness.CreateAccount(userId, _account);

        Assert.That(result.Item1, Is.True);
    }

    [Test]
    public void CreateAccount_ServerFails_ThrowsException()
    {
        int userId = 1;
        _unitOfWork.Setup(repo => repo.accountRepository.AddAsync(It.IsAny<Account>())).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountBusiness.CreateAccount(userId, _account));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.CreateAccount));
    }
}