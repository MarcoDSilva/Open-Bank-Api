using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.DTO;
using OpenBank.API.Domain.Entities;
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

    /* CONTROL TEST to validate the logic and cruds. Real one will use a token */
    [HttpPost]
    public async Task<IActionResult> Accounts(CreateAccountRequest accountRequest)
    {
        var authToken = HttpContext.Request.Headers["Authorization"];
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        // validar campos vazios
        if (userId <= 0)
        {
            return Unauthorized("Accout is not valid or does not exist.");
        }

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
        var authToken = HttpContext.Request.Headers["Authorization"];
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized("You must login first");

        try
        {
            var result = await _unitOfWork.accountRepository.GetAccounts(userId);

            if (result == null || result.Count == 0)
                return NotFound("Account not found.");

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
        var authToken = HttpContext.Request.Headers["Authorization"];
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized("You must login first");
        
        // validar campos vazios
        if (id <= 0)
        {
            return Problem("id must be higher than 0");
        }

        try
        {
            Account account = await _unitOfWork.accountRepository.GetAccountById(id);

            if (account is null)
                return NotFound("There is no account with this ID");

            AccountMovim accountWithMovements = await _unitOfWork.accountRepository.GetAccountMovements(account);

            return Ok(accountWithMovements);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}
