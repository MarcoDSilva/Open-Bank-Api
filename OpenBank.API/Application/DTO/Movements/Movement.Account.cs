namespace OpenBank.API.Application.DTO;

public class AccountMovement
{
    public AccountResponse Account { get; set; }
    public List<MovementResponse> Movements { get; set; }

}
