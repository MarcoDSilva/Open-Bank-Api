using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using OpenBank.Api.Shared;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Application.Services.Logic;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.UnitTesting;

[TestFixture]
public class TransferServiceTests
{
    private ITransferService _transferBusiness;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Mock<IConfiguration> _config;

    private TransferRequest _movement;
    private Account _From_account;
    private Account _To_account;

    public TransferServiceTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _config = new Mock<IConfiguration>();
        _transferBusiness = new TransferService(_unitOfWork.Object, _mapper.Object, _config.Object);

        SetUp();
    }

    [SetUp]
    public void SetUp()
    {
        _movement = new TransferRequest()
        {
            From_account = 1,
            To_account = 2,
            Amount = 5
        };

        _From_account = SetAccount(1);
        _To_account = SetAccount(2);

        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_From_account.Id)).ReturnsAsync(_From_account);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_To_account.Id)).ReturnsAsync(_To_account);

        _unitOfWork.Setup(t => t.transferRepository.SaveAsync()).ReturnsAsync(true);
    }

    private Account SetAccount(int id)
    {
        return new Account()
        {

            Balance = 10,
            Created_at = DateTime.UtcNow,
            Currency = "EUR",
            Id = id,
            UserId = id
        };
    }

    [Test]
    public async Task TransferRequest_CorrectRequest_ReturnsOk()
    {
        var result = await _transferBusiness.TransferRequestAsync(_movement, _From_account.UserId);

        Assert.That(result, Is.EqualTo("Transfer was completed with success"));
    }

    [Test]
    public void TransferRequest_UserIsNotTheOwner_ThrowsForbiddenAccountAcessException()
    {
        int fakeUserId = 3;

        var result = Assert.ThrowsAsync<ForbiddenAccountAccessException>(async () => await _transferBusiness.TransferRequestAsync(_movement, fakeUserId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(AccountDescriptions.BearerNotAllowed));
    }


    [Test]
    public void TransferRequest_DifferentCurrenciesBetweenAccounts_ThrowsDifferentCurrenciesException()
    {
        _From_account.Currency = "USD";
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_From_account.Id)).ReturnsAsync(_From_account);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_To_account.Id)).ReturnsAsync(_To_account);

        var result = Assert.ThrowsAsync<DifferentCurrenciesException>(async () => await _transferBusiness.TransferRequestAsync(_movement, _From_account.UserId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(AccountDescriptions.DifferentCurrencies));
    }

    [Test]
    public void TransferRequest_AmountFromAccountIsLessThanTheRequested_ReturnsBadRequest()
    {
        _movement.Amount = 11;

        var result = Assert.ThrowsAsync<LowerBalanceException>(async () => await _transferBusiness.TransferRequestAsync(_movement, _From_account.UserId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(AccountDescriptions.LowerBalance));
    }




    [Test]
    public void TransferRequest_ServerFails_ThrowsException()
    {
        _unitOfWork.Setup(acc => acc.accountRepository.Update(_From_account)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _transferBusiness.TransferRequestAsync(_movement, _From_account.UserId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.FailedTransfer));
    }
}