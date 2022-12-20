namespace OpenBank.API.Infrastructure.DTO;

public class CreateUserResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime PasswordChangedAt { get; set; }
    public string Username { get; set; }
}