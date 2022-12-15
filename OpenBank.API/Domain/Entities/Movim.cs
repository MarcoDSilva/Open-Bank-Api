using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenBank.API.Domain.Entities;

public class Movim
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(1.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public decimal Balance { get; set; }

    [Required]
    public DateTime Created_at { get; set; }

    [Required]
    [ForeignKey("Accounts")]
    public int AccountId { get; set; }
    public Account Account { get; set; }
}
