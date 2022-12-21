namespace OpenBank.API.Infrastructure.DTO;

public class AccountResponse
{
    public int Id { get; set; }

    public decimal Balance { get; set; }

    public DateTime Created_at { get; set; }

    public string Currency { get; set; }
}