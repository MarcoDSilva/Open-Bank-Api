using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly OpenBankApiDbContext _openBankApiDbContext;

    public DocumentRepository(OpenBankApiDbContext openBankApiDbContext)
    {
        _openBankApiDbContext = openBankApiDbContext;
    }

    public async Task<Document?> AddAsync(Document document)
    {
        Document? failedDocument = null;
        var createdDocument = await _openBankApiDbContext.Documents.AddAsync(document);
        var saved = await _openBankApiDbContext.SaveChangesAsync();

        return saved > 0 ? createdDocument.Entity : failedDocument;
    }

    public async Task<Document?> GetDocumentAsync(int documentId)
    {
        List<Document> documents = await _openBankApiDbContext.Documents.ToListAsync();
        Document? document = documents.Find(doc => doc.Id == documentId);

        return document;
    }

    public async Task<List<Document>> GetDocumentsAsync(int accountId)
    {
        List<Document> documents = await _openBankApiDbContext.Documents.ToListAsync();
        List<Document> userDocuments = documents.FindAll(doc => doc.AccountId == accountId);

        return userDocuments;
    }
}