using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> AddAsync(Document document);
    Task<List<Document>> GetDocumentsAsync(int accountId);
    Task<Document?> GetDocumentAsync(int documentId);
}