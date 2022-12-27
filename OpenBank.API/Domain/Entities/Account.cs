using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OpenBank.API.Domain.Common;

namespace OpenBank.API.Domain.Entities;

public class Account : BaseEntity
{
    [Required]
    public decimal Balance { get; set; }

    // The plan is to turn currency into either a table or a enum    
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string Currency { get; set; }

    [ForeignKey("Users")]
    public int UserId { get; set; }
}