using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Interfaces;

public interface IDocumentBusinessRules
{
    Task<Document?> AddAsync(Document document);
    Task<List<Document>> GetDocumentsAsync(int accountId);
    Task<Document?> GetDocumentAsync(int docId);

}