using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Logic;

public class DocumentBusinessRules : IDocumentBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;

    public DocumentBusinessRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Document> GetDocument(int accountId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Document>> GetDocumentsAsync(int accountId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SubmitDocumentAsync()
    {
        throw new NotImplementedException();
    }
}