using System.ComponentModel.DataAnnotations;
using OpenBank.API.Domain.Common;

namespace OpenBank.API.Domain.Entities;

public class Transfer : BaseEntity
{
    [Required]
    [Range(1.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public decimal Amount { get; set; }

    [Required]
    public int From_account { get; set; }

    [Required]
    public int To_account { get; set; }
}
