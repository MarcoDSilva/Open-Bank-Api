using Moq;
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
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_accountFrom.Id)).ReturnsAsync(_accountFrom);
        _unitOfWork.Setup(acc => acc.accountRepository.GetByIdAsync(_accountTo.Id)).ReturnsAsync(_accountTo);
        _unitOfWork.Setup(t => t.transferRepository.SaveAsync()).ReturnsAsync(true);

        var result = await _transferBusiness.TransferRequestAsync(_movement, _accountFrom.UserId);

        Assert.That(result.Item1, Is.EqualTo(StatusCode.Sucess));
        Assert.That(result.Item2, Is.EqualTo("Transfer was completed with success"));
    }

    [Test]
    [Ignore("not implemented")]
    public void TransferRequest_UserIsNotTheOwner_ReturnsBadRequest()
    {

    }

    [Test]
    [Ignore("not implemented")]
    public void TransferRequest_DifferentCurrenciesBetweenAccounts_ReturnsBadRequest()
    {

    }

    [Test]
    [Ignore("not implemented")]
    public void TransferRequest_AmountFromAccountIsLessThanTheRequested_ReturnsBadRequest()
    {

    }

    [Test]
    [Ignore("not implemented")]
    public void TransferRequest_ServerFails_ThrowsException()
    {

    }
}