using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Application.DTO;

public class CreateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string Email { get; set; }
    
    [Required]
    [MinLength(16)]
    [MaxLength(250)]
    public string FullName { get; set; }
    
    [Required]
    [MinLength(8)]
    [MaxLength(500)]
    public string Password { get; set; }
    
    [Required]    
    [MinLength(8)]
    [MaxLength(50)]
    public string Username { get; set; }
}