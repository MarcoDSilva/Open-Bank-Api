using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Application;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]

public class DocumentsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentBusinessRules _documentBusiness;

    public DocumentsController(IUnitOfWork unitOfWork, IDocumentBusinessRules documentBusinessRules)
    {
        _unitOfWork = unitOfWork;
        _documentBusiness = documentBusinessRules;
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
            return Unauthorized("You must login first");

        return NotFound();
    }

    [HttpGet("accounts/{accountId}/documents")]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAccountDocuments([FromRoute] int accoundId)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized("You must login first");

        return NotFound();
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
            return Unauthorized("You must login first");

        return NotFound();
    }

}
