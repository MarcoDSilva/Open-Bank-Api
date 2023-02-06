using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    public LoginRequest(string username, string password)
    {
        this.UserName = username;
        this.Password = password;
    }

    [Required]
    [MinLength(8)]
    public string UserName;

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}