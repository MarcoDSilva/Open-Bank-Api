using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.DTO;
using Microsoft.AspNetCore.Authorization;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> Accounts(CreateAccountRequest accountRequest)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        // validar campos vazios
        if (userId <= 0)
            return Unauthorized("Accout is not valid or does not exist.");

        try
        {
            var result = await _unitOfWork.accountRepository.CreateAccount(userId, accountRequest);
            return Ok(result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpGet]
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
            return Problem(e.Message);
        }
    }


    [HttpGet]
    [Route("{id}")]

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
            return BadRequest("id must be higher than 0");

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
            return Forbid(ex.Message);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}
