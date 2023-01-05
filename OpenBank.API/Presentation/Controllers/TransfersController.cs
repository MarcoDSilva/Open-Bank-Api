using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBank.Api.Shared;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransfersController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransferBusinessRules _transferBusiness;
    private readonly IAccountBusinessRules _accountBusiness;

    public TransfersController(IUnitOfWork unitOfWork, ITransferBusinessRules transferBusiness, IAccountBusinessRules accountBusiness)
    {
        _unitOfWork = unitOfWork;
        _transferBusiness = transferBusiness;
        _accountBusiness = accountBusiness;
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
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        try
        {
            Movement movement = new Movement()
            {
                accountFrom = transferRequest.From_account,
                accountTo = transferRequest.To_account,
                Amount = transferRequest.Amount
            };

            var result = await _transferBusiness.TransferRequestAsync(movement, userId);

            switch (result.Item1)
            {
                case Enum.StatusCode.Sucess:
                    return Ok(result.Item2);
                case Enum.StatusCode.BadRequest:
                    return BadRequest(result.Item2);
                case Enum.StatusCode.NotFound:
                    return NotFound(result.Item2);
                case Enum.StatusCode.Forbidden:
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
