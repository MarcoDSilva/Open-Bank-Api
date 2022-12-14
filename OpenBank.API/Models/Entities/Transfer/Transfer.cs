using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Models.Entities;

public class Transfer
{
    [Key]
    public int Id {get; set;}

    [Required]
    [Range(0.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public decimal Amount { get; set; }

    [Required]
    public int From_account { get; set; }

    [Required]
    public int To_account { get; set; }
}