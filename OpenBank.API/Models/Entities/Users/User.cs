using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

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