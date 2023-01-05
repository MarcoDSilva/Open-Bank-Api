using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Interfaces;

public interface IDocumentBusinessRules
{
    Task<bool> SubmitDocumentAsync();
    Task<List<Document>> GetDocumentsAsync(int accountId, int userId);
    Task<Document> GetDocument(int accountId, int userId);

}