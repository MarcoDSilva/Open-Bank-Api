namespace OpenBank.API.Domain.Models.Entities;

public class Movement
{
    public int accountFrom { get; set; }
    public int accountTo { get; set; }
    public decimal Amount { get; set; }
}