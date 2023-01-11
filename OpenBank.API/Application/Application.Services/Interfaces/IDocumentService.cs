using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Interfaces;

public interface IDocumentService
{
    Task<Document?> AddAsync(Document document);
    Task<List<Document>> GetDocumentsAsync(int accountId);
    Task<Document?> GetDocumentAsync(int docId);

}