using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Models.Entities;

public class Movim
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(1.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public decimal Balance { get; set; }
    
    [Required]
    public DateTime Created_at { get; set; }
}
