using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Models.Entities;

public class TransferRequest
{
    [Required]
    [Range(0.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public decimal Amount { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "The number account must be greater than 0.")]
    public int From_account { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "The number account must be greater than 0.")]
    public int To_account { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string Currency { get; set; }
}