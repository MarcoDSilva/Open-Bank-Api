using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Created_at { get; set; }    
}
