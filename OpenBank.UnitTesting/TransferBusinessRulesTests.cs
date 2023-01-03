using NUnit.Framework;

namespace OpenBank.UnitTesting;

[TestFixture]
public class TransferBusinessRulesTests
{
    [SetUp]
    public void SetUp()
    {

    }

    [Test]
    [Ignore("not implemented")]
    public void TransferRequest_CorrectRequest_ReturnsOk()
    {

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