namespace OpenBank.API.Application.DTO;

public class DocumentResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public DateTime Created_at { get; set; }
}