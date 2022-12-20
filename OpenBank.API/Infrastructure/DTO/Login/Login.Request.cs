using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Infrastructure.DTO;

public class LoginUserRequest
{
    [Required]
    [MinLength(8)]
    public string UserName { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}