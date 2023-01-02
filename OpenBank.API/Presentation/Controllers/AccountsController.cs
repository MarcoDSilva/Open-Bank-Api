using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using OpenBank.API.BusinessRules.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountBusinessRules _accountBusinessRules;

    public AccountsController(IUnitOfWork unitOfWork, IAccountBusinessRules accountBusinessRules)
    {
        _unitOfWork = unitOfWork;
        _accountBusinessRules = accountBusinessRules;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<CreateAccountRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Accounts(CreateAccountRequest accountRequest)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        // validar campos vazios
        if (userId <= 0)
            return Unauthorized("Accout is not valid or does not exist.");

        try
        {
            var result = await _accountBusinessRules.CreateAccount(userId, accountRequest);

            return !result.Item1 ? Problem() : Ok(result.Item2);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Accounts with the message: {e.Message}");
            return Problem(e.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    /// <summary>
    /// Gets all the accounts that the logged in user has
    /// </summary>
    public async Task<IActionResult> Accounts()
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized("You must login first");

        try
        {
            List<AccountResponse> result = await _accountBusinessRules.GetAccounts(userId);

            if (result == null || result.Count == 0)
                return NotFound("This user has no accounts.");

            return Ok(result);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Accounts with the message: {e.Message}");
            return Problem(e.Message);
        }
    }


    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(IEnumerable<AccountMovement>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

    /// <summary>
    /// Get the account and movements associated with the id
    /// </summary>
    public async Task<IActionResult> Accounts(int id)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized("You must login first");

        // validar campos vazios
        if (id <= 0)
            return BadRequest("Account id must be higher than 0");

        try
        {
            AccountResponse? account = await _accountBusinessRules.GetAccountById(id, userId);

            if (account is null)
                return NotFound("There is no account with this ID");

            List<MovementResponse> movements = await _accountBusinessRules.GetAccountMovements(account.Id);            
            AccountMovement accountWithMovements = new AccountMovement()
            {
                Account = account,
                Movements = movements
            };

            return Ok(accountWithMovements);
        }
        catch (ForbiddenAccountAccessException ex)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Information, $"Forbidden user: {ex.Message}");
            return Forbid("Bearer");
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Accounts with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
