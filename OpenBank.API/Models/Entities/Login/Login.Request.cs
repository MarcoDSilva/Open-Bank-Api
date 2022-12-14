using System.ComponentModel.DataAnnotations;

namespace OpenBank.API.Models.Entities;

public class LoginUserRequest
{
    [Required]
    [MinLength(8)]
    public string UserName { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}