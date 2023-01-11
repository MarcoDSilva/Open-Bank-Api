using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;
using AutoMapper;
using OpenBank.Api.Shared;
using Microsoft.AspNetCore.Authorization;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
[Authorize]
public class DocumentsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentService _documentBusiness;
    private readonly IAccountService _accountBusiness;

    public DocumentsController(IUnitOfWork unitOfWork, IDocumentService documentBusinessRules, IMapper mapper, IAccountService accountRules)
    {
        _unitOfWork = unitOfWork;
        _documentBusiness = documentBusinessRules;
        _mapper = mapper;
        _accountBusiness = accountRules;
    }

    [HttpPost("accounts/{accountId}/documents")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitDocument([FromRoute] int accountId, IFormFile uploadedFile)
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

        if (uploadedFile is null)
            return BadRequest("File not uploaded");

        string mime = MimeTypes.GetMimeType(uploadedFile.FileName);

        if (!mime.Equals("application/pdf") && !mime.Equals("application/png"))
            return BadRequest("Document can only be of .png or .pdf");

        if (uploadedFile.Length > 2000000)
            return BadRequest("File cannot be bigger than 2MB");

        string rootDir = Directory.GetDirectoryRoot("OpenBank");
        string folderPath = string.Concat(rootDir, "ApiUserFiles\\", "Accounts\\", account.Id.ToString() + "\\", "Documents\\");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileCreationUrl = string.Concat(
            folderPath,
            Guid.NewGuid().ToString(),
            mime == "application/pdf" ? ".pdf" : ".png");

        try
        {
            if (!System.IO.File.Exists(fileCreationUrl))
            {
                using (Stream fs = new FileStream(fileCreationUrl, FileMode.Create))
                    await uploadedFile.CopyToAsync(fs);
            }

            if (System.IO.File.Exists(fileCreationUrl))
            {
                Document documentToInsert = new Document()
                {
                    AccountId = accountId,
                    ContentType = uploadedFile.ContentType,
                    Created_at = DateTime.UtcNow,
                    FileName = uploadedFile.FileName,
                    Url = fileCreationUrl,
                    SizeMB = uploadedFile.Length / Math.Pow(1024, 2)
                };

                var saved = await _documentBusiness.AddAsync(documentToInsert);

                if (saved?.Id > 0)
                    return Ok(_mapper.Map<Document, DocumentResponse>(saved));
            }

            return Problem("Could not upload/create the file");
        }
        catch (System.Exception)
        {
            return Problem("fail");
        }
    }

    [HttpGet("accounts/{accountId:int}/documents")]
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

    [HttpGet("accounts/{accountId:int}/documents/{docId:int}")]
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
            Document? document = await _documentBusiness.GetDocumentAsync(docId);

            if (document is null)
                return Ok("Document not found");

            if (!System.IO.File.Exists(document.Url))
                return Problem("Document not found");


            return new FileStreamResult(System.IO.File.OpenRead(document.Url), document.ContentType)
            {
                FileDownloadName = document.FileName
            };
           
            //return Ok(documentDTO);
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetDocument error: {e.Message}");
            return Problem(e.Message);
        }
    }

}
