using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Domain.Entities;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    public decimal Balance { get; set; }

    [Required]
    public DateTime Created_at { get; set; }

    // The plan is to turn currency into either a table or a enum    
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string Currency { get; set; }

    //public Currency Currency { get; set; }

    [ForeignKey("Users")]
    public int UserId { get; set; }
}