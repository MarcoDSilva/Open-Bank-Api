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
    private readonly ITransferService _transferServices;
    private readonly IAccountService _accountServices;
    private readonly ITokenService _tokenServices;

    public TransfersController(ITransferService transferServices, IAccountService accountServices, ITokenService tokenServices)
    {
        _transferServices = transferServices;
        _accountServices = accountServices;
        _tokenServices = tokenServices;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Transfers(TransferRequest transferRequest)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _tokenServices.GetUserIdFromToken(authToken);

        if (userId <= 0)
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        try
        {
            var result = await _transferServices.TransferRequestAsync(transferRequest, userId);
            return Ok(result);

        }
        catch (Exception e)
        {
            // _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Transfers with the message: {e.Message}");

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
