namespace OpenBank.API.Models.Entities;

public class TransferRequest
{
    public decimal Amount { get; set; }
    public int From_account { get; set; }
    public int To_account { get; set; }
    public string Currency { get; set; }
}
