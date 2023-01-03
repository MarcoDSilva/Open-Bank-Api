using System.ComponentModel.DataAnnotations;
using OpenBank.API.Domain.Models.Common;

namespace OpenBank.API.Domain.Models.Entities;

public class User : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(250)]
    public string FullName { get; set; }

    [Required]
    [StringLength(500)]
    public string Password { get; set; }

    [Required]
    [StringLength(50)]
    public string UserName { get; set; }
}