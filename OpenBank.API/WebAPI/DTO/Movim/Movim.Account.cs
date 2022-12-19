namespace OpenBank.API.DTO;

using OpenBank.API.Domain.Entities;

public class AccountMovim
{
    public Account Account { get; set; }
    public List<MovimResponse> Movimentos { get; set; }

}
