using System.ComponentModel.DataAnnotations;

public class CreateUserRequest
{

    public CreateUserRequest(string email, string fullname, string password, string username)
    {
        this.Email = email;
        this.FullName = fullname;
        this.Password = password;
        this.Username = username;
    }

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