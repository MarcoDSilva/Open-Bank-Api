using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Models.Entities;

public class CreateAccountRequest
{
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string Currency { get; set; }
}