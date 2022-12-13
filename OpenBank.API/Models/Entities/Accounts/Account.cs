namespace OpenBank.API.Models.Entities;

public class Account
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public DateTime Created_at { get; set; }

    // The plan is to turn currency into either a table or a enum
    public string Currency { get; set; }

    //public Currency Currency { get; set; }
    public int User_Id { get; set; }
}