using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using OpenBank.API.BusinessLogic;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AccountBusinessRules _accountBusinessRules;

    public AccountsController(IUnitOfWork unitOfWork, AccountBusinessRules accountBusinessRules )
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

            if (!result.Item1)
                return Problem();

            return Ok(result.Item2);

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
            List<AccountResponse> result = await _unitOfWork.accountRepository.GetAccounts(userId);

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
    [ProducesResponseType(typeof(IEnumerable<AccountMovim>), StatusCodes.Status200OK)]
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
            AccountResponse account = await _unitOfWork.accountRepository.GetAccountById(id, userId);

            if (account is null)
                return NotFound("There is no account with this ID");

            AccountMovim accountWithMovements = await _unitOfWork.accountRepository.GetAccountMovements(account);

            return Ok(accountWithMovements);
        }
        catch (ForbiddenAccountAccessException ex)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Information, $"Exception caught on controller Accounts with the message: {ex.Message}");
            return Forbid(ex.Message);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Accounts with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
