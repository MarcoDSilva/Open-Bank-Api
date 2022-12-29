using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.BusinessLogic.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransfersController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransferBusinessRules _transferBusinessRules;

    public TransfersController(IUnitOfWork unitOfWork, ITransferBusinessRules transferBusinessRules)
    {
        _unitOfWork = unitOfWork;
        _transferBusinessRules = transferBusinessRules;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Transfers(TransferRequest transferRequest)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized("You must login first");

        try
        {
            var result = await _transferBusinessRules.TransferRequestAsync(transferRequest, userId);

            switch (result.Item1)
            {
                case Application.Enum.StatusCode.Sucess:
                    return Ok(result.Item2);
                case Application.Enum.StatusCode.BadRequest:
                    return BadRequest(result.Item2);
                case Application.Enum.StatusCode.NotFound:
                    return NotFound(result.Item2);
                case Application.Enum.StatusCode.Forbidden:
                    return Forbid(result.Item2);
                default:
                    return Ok(result.Item2);
            }
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Transfers with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
