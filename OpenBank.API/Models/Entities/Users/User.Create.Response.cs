namespace OpenBank.API.Models.Entities;

public class CreateUserResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PasswordChangedAt { get; set; }
    public string Username { get; set; }
}