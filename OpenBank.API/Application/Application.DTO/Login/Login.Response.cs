namespace OpenBank.API.Application.DTO;

public class LoginUserResponse
{
    public string AcessToken { get; set; }
    public string AcessTokenExpires { get; set; }
    public string SessionId { get; set; }
}