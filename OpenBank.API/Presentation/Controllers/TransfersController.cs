using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBank.Api.Shared;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransfersController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransferService _transferBusiness;
    private readonly IAccountService _accountBusiness;

    public TransfersController(IUnitOfWork unitOfWork, ITransferService transferBusiness, IAccountService accountBusiness)
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
            var result = await _transferBusiness.TransferRequestAsync(transferRequest, userId);
            return Ok(result);

        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Transfers with the message: {e.Message}");

            switch (e)
            {
                case MovementAccountNotFoundException:
                    return NotFound(e.Message);
                case ForbiddenAccountAccessException:
                    return Forbid(e.Message);
                case LowerBalanceException:
                case DifferentCurrenciesException:
                    return BadRequest(e.Message);
                default:
                    return Problem(e.Message);
            }
        }
    }
}
