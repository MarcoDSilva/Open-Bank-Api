using Org.BouncyCastle.Asn1.Crmf;

namespace OpenBank.Infrastructure.Email.Model;

public class EmailBase
{
    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Body { get; set; }
}