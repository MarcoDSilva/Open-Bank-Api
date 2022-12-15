namespace OpenBank.API.DTO;


public class LoginUserResponse
{
    public string AcessToken { get; set; }
    public string AcessTokenExpires { get; set; }
    public string SessionId { get; set; }
    public CreateUserResponse user{ get; set; }
}