using System.Net.NetworkInformation;
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

    public async Task<Document?> GetDocumentAsync(int accountId)
    {
        Document? document = await _unitOfWork.documentRepository.GetDocumentAsync(accountId);
        return document;
    }

    public async Task<List<Document>> GetDocumentsAsync(int accountId)
    {
        List<Document> documents = await _unitOfWork.documentRepository.GetDocumentsAsync(accountId);
        return documents;
    }

    public async Task<Document?> AddAsync(Document doc)
    {
        Document? document = await _unitOfWork.documentRepository.AddAsync(doc);
        return document;
    }
}