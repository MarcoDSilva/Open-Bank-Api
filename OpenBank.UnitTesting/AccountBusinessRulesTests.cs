using AutoMapper;
using Microsoft.OpenApi.Any;
using Moq;
using NUnit.Framework;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.Repositories;
using OpenBank.API.BusinessRules;
using OpenBank.API.BusinessRules.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.UnitTesting;

[TestFixture]
public class AccountBusinessRulesTests
{
    private IAccountBusinessRules _accountBusiness;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private CreateAccountRequest _createAccount;

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
        _createAccount = new CreateAccountRequest()
        {
            Amount = 100,
            Currency = "EUR"
        };

    }

    [Test]
    [Ignore("not implemented")]
    public void GetAccounts_ReceivesUserID_ReturnsAccounts()
    {
    }

    [Test]
    [Ignore("not implemented")]
    public void GetAccounts_ServerFails_ThrowsException()
    {

    }

    [Test]
    [Ignore("not implemented")]
    public void GetAccountById_ReceivesAccountId_ReturnsAccount()
    {

    }

    [Test]
    [Ignore("not implemented")]
    public void GetAccountById_ReceivesNonExistantId_ReturnsNothing()
    {

    }

    [Test]
    public async Task CreateAccount_GetsParametersToCreateAcc_ReturnsCreatedAccount()
    {
        _unitOfWork.Setup(creation => creation.accountRepository.AddAsync(It.IsAny<Account>())).ReturnsAsync(1);

        var result = await _accountBusiness.CreateAccount(1, _createAccount);
        Assert.That(result.Item1, Is.True);
    }

    [Test]
    public async Task CreateAccount_ServerFails_ThrowsException()
    {
        _unitOfWork.Setup(creation => creation.accountRepository.AddAsync(It.IsAny<Account>())).ThrowsAsync(new ArgumentException());
        var result = await _accountBusiness.CreateAccount(1, _createAccount);

        Assert.ThrowsAsync<ArgumentException>(async () => await _accountBusiness.CreateAccount(4, _createAccount));
    }
}