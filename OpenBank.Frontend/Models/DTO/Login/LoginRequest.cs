using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required]
    [MinLength(8, ErrorMessage = "Username is too short.")]
    public string username { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "password is too short.")]
    public string password { get; set; }
}