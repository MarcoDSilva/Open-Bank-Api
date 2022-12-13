namespace OpenBank.API.Models.Entities;

public class CreateAccountRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}