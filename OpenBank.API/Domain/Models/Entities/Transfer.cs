using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenBank.API.Domain.Models.Common;

namespace OpenBank.API.Domain.Models.Entities;

public class Transfer : BaseEntity
{
    [Required]
    public string OperationType {get; set;}

    [Required]
    [Range(1.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public decimal Amount { get; set; }

    [Required]
    [ForeignKey("Accounts")]
    public int AccountId { get; set; }
    public Account Account { get; set; }
}
