namespace OpenBank.API.Application.DTO;

public class MovementResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Created_at { get; set; }
    public string OperationType { get; set; }
}
