namespace OpenBank.API.Application.DTO;

public class TransferCommunication
{
    public Guid id { get; set; }
    public UserCommunicationEmail fromUser { get; set; }
    public UserCommunicationEmail toUser { get; set; }
    public decimal amount { get; set; }
    public string currency { get; set; }
}
