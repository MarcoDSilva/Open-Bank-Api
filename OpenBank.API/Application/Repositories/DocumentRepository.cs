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

    public Task<Document> AddAsync(Document document)
    {
        throw new NotImplementedException();
    }

    public Task<Document> GetDocumentAsync(int accountId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Document>> GetDocumentsAsync(int accountId)
    {
        throw new NotImplementedException();
    }
}