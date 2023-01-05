using Moq;
using OpenBank.Api.Shared;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Business.Logic;
using OpenBank.API.Domain.Models.Entities;
using OpenBank.API.Enum;

namespace OpenBank.UnitTesting;

[TestFixture]
public class TransferBusinessRulesTests
{
    private ITransferBusinessRules _transferBusiness;
    private Mock<IUnitOfWork> _unitOfWork;
    private Movement _movement;
    private Account _accountFrom;
    private Account _accountTo;

    public TransferBusinessRulesTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _transferBusiness = new TransferBusinessRules(_unitOfWork.Object);

        SetUp();
    }

    [SetUp]
    public void SetUp()
    {
        _movement = new Movement()
        {
            accountFrom = 1,
            accountTo = 2,
            Amount = 5
        };

        _accountFrom = SetAccount(1);
        _accountTo = SetAccount(2);

        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_accountFrom.Id)).ReturnsAsync(_accountFrom);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_accountTo.Id)).ReturnsAsync(_accountTo);

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
        var result = await _transferBusiness.TransferRequestAsync(_movement, _accountFrom.UserId);

        Assert.That(result.Item1, Is.EqualTo(StatusCode.Sucess));
        Assert.That(result.Item2, Is.EqualTo("Transfer was completed with success"));
    }

    [Test]
    public async Task TransferRequest_UserIsNotTheOwner_ReturnsBadRequest()
    {
        int fakeUserId = 3;
        
        var result = await _transferBusiness.TransferRequestAsync(_movement, fakeUserId);

        Assert.That(result.Item1, Is.EqualTo(StatusCode.Forbidden));
        Assert.That(result.Item2, Is.EqualTo(AccountDescriptions.BearerNotAllowed));
    }


    [Test]
    public async Task TransferRequest_DifferentCurrenciesBetweenAccounts_ReturnsBadRequest()
    {
        _accountFrom.Currency = "USD";
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_accountFrom.Id)).ReturnsAsync(_accountFrom);

        var result = await _transferBusiness.TransferRequestAsync(_movement, _accountFrom.UserId);

        Assert.That(result.Item1, Is.EqualTo(StatusCode.BadRequest));
        Assert.That(result.Item2, Is.EqualTo(AccountDescriptions.DifferentCurrencies));
    }

    [Test]
    public async Task TransferRequest_AmountFromAccountIsLessThanTheRequested_ReturnsBadRequest()
    {
        _movement.Amount = 11;

        var result = await _transferBusiness.TransferRequestAsync(_movement, _accountFrom.UserId);

        Assert.That(result.Item1, Is.EqualTo(StatusCode.BadRequest));
        Assert.That(result.Item2, Is.EqualTo(AccountDescriptions.LowerBalance));
    }

    [Test]
    public void TransferRequest_ServerFails_ThrowsException()
    {
        _unitOfWork.Setup(acc => acc.accountRepository.Update(_accountFrom)).Throws(new Exception());

        var result = Assert.ThrowsAsync<Exception>(async () => await _transferBusiness.TransferRequestAsync(_movement, _accountFrom.UserId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Message, Is.EqualTo(WarningDescriptions.FailedTransfer));
    }
}