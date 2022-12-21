namespace OpenBank.API.Infrastructure.DTO;

using OpenBank.API.Domain.Entities;

public class AccountMovim
{
    public AccountResponse Account { get; set; }
    public List<MovimResponse> Movimentos { get; set; }

}
