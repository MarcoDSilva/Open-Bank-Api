using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Logic;

public class DocumentBusinessRules : IDocumentBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;

    public DocumentBusinessRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Document?> GetDocumentAsync(int docId)
    {
        var document = await _unitOfWork.documentRepository.GetDocumentAsync(docId);
        return document;
    }

    public async Task<List<Document>> GetDocumentsAsync(int accountId)
    {
        var documents = await _unitOfWork.documentRepository.GetDocumentsAsync(accountId);
        return documents;
    }

    public async Task<Document?> AddAsync(Document doc)
    {
       var document = await _unitOfWork.documentRepository.AddAsync(doc);
        return document;
    }
}