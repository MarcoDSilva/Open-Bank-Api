namespace OpenBank.API.Models.Entities;

public class AccountMovim
{
    public Account Account { get; set; }
    public List<Account> Movimentos { get; set; }

}
