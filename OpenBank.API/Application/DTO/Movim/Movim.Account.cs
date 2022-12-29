namespace OpenBank.API.Application.DTO;

public class AccountMovim
{
    public AccountResponse Account { get; set; }
    public List<MovimResponse> Movimentos { get; set; }

}
