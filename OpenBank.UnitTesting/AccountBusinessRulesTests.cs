using AutoMapper;
using Moq;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Logic;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;

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
        Assert.That(result?.Message, Is.EqualTo("Error while obtaining the account"));
    }

    [Test]
    public void GetAccountById_ReceivesAccountId_ReturnsAccount()
    {
        int userId = 1;
        int accountId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.IsUserAccountAsync(1, 1)).ReturnsAsync(true);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).ReturnsAsync(_account);

        var result = _accountBusiness.GetAccountById(accountId, userId);

        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.Result?.Id, accountId);
        Assert.AreEqual(result.Result?.UserId, userId);
        Assert.AreEqual(result.Result?.Currency, _account.Currency);
    }

    [Test]
    public void GetAccountById_ReceivesNonExistantId_ReturnsNull()
    {
        int userId = 1;
        int accountId = 1;
        Account? accountIsNull = null;

        _unitOfWork.Setup(acc => acc.accountRepository.IsUserAccountAsync(1, 1)).ReturnsAsync(true);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).ReturnsAsync(accountIsNull);

        var result = _accountBusiness.GetAccountById(accountId, userId);

        Assert.That(result.Result, Is.Null);
    }

    public void GetAccountById_ServerFails_ThrowsException()
    {
        int userId = 1;
        int accountId = 1;

        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(accountId, userId)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _accountBusiness.GetAccountById(accountId, userId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo("Error while obtaining this account"));
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
        Assert.That(result?.Message, Is.EqualTo("Error while creating the user"));
    }
}