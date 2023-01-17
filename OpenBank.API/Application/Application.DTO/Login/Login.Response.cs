namespace OpenBank.API.Application.DTO;

public class LoginUserResponse
{
    public string AcessToken { get; set; }
    public string AcessTokenExpires { get; set; }
    public string RefreshToken { get; set; }
    public string RefreshTokenExpires { get; set; }
    public string SessionId { get; set; }
}