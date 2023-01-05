using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Application;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Mime;
using OpenBank.API.Domain.Models.Entities;
using AutoMapper;
using OpenBank.Api.Shared;
using Microsoft.AspNetCore.Identity;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]

public class DocumentsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentBusinessRules _documentBusiness;
    private readonly IAccountBusinessRules _accountBusiness;

    public DocumentsController(IUnitOfWork unitOfWork, IDocumentBusinessRules documentBusinessRules, IMapper mapper, IAccountBusinessRules accountRules)
    {
        _unitOfWork = unitOfWork;
        _documentBusiness = documentBusinessRules;
        _mapper = mapper;
        _accountBusiness = accountRules;
    }

    [HttpPost("accounts/{accoundId}/documents")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitDocument([FromRoute] int accountId)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        Account? account = await _accountBusiness.GetAccountById(accountId, userId);

        if (account is null)
            return NotFound(AccountDescriptions.AccountNonExistant);

        if (account.UserId != userId)
            return Forbid(WarningDescriptions.ForbiddenAccess);

        // get document bytes
        // turn into a file 
        // place it on a folder
        // write document obj and insert in db
        // problem() if any of the steps fail

        try
        {
            Document? documentInserted = await _documentBusiness.AddAsync(new Document());

            if (documentInserted is null)
                return Problem();

            return Ok("Successfully added");
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetAccountDocuments error: {e.Message}");
            return Problem(e.Message);
        }
    }

    [HttpGet("accounts/{accountId}/documents")]
    [ProducesResponseType(typeof(IEnumerable<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAccountDocuments([FromRoute] int accountId)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        Account? account = await _accountBusiness.GetAccountById(accountId, userId);

        if (account is null)
            return NotFound(AccountDescriptions.AccountNonExistant);

        if (account.UserId != userId)
            return Forbid(WarningDescriptions.ForbiddenAccess);

        try
        {
            List<Document> documents = await _documentBusiness.GetDocumentsAsync(accountId);

            if (documents.Count <= 0)
                return Ok(documents);

            List<DocumentResponse> documentsDTO = new List<DocumentResponse>();
            documents.ForEach(doc => documentsDTO.Add(_mapper.Map<Document, DocumentResponse>(doc)));

            return Ok(documentsDTO);
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetAccountDocuments error: {e.Message}");
            return Problem(e.Message);
        }
    }

    [HttpGet("accounts/{accountId}/documents/{docId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDocument([FromRoute] int accountId, [FromRoute] int docId)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        Account? account = await _accountBusiness.GetAccountById(accountId, userId);

        if (account is null)
            return NotFound(AccountDescriptions.AccountNonExistant);

        if (account.UserId != userId)
            return Forbid(WarningDescriptions.ForbiddenAccess);

        try
        {
            Document? document = await _documentBusiness.GetDocumentAsync(accountId);

            if (document is null)
                return Ok("Document not found");

            DocumentResponse documentDTO = _mapper.Map<Document, DocumentResponse>(document);
            return Ok(documentDTO);
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetDocument error: {e.Message}");
            return Problem(e.Message);
        }
    }

}
