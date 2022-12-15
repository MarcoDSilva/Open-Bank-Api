using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.DTO;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /* CONTROL TEST to validate the logic and cruds. Real one will use a token */
    // [HttpGet]
    // [Route("accounts")]
    // public IActionResult Accounts(User user)
    // {
    //     return Ok();
    // }

    /* CONTROL TEST to validate the logic and cruds. Real one will use a token */
    [HttpPost]
    public IActionResult Accounts(int idUser, CreateAccountRequest accountRequest)
    {
        // validar campos vazios
        if (idUser <= 0)
        {
            return Problem("ID must be a number higher than 0");
        }

        try
        {
            var result = _unitOfWork.accountRepository.CreateAccount(idUser, accountRequest);
            return Ok(result.Result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    /*
        Create account
    */
    // [HttpPost]
    // [Route("accounts")]
    // public IActionResult Accounts(CreateAccountRequest accountRequest)
    // {
    //     // validar campos vazios
    //     var result = _accountRepository.CreateAccount(accountRequest);
    //     return Ok();
    // }

    [HttpGet]
    /// <summary>
    /// WARNING: This is the regular Accounts() Controller, which will use the Token to validate the user and then
    /// show all the associated accounts
    /// </summary>
    public IActionResult Accounts(int id)
    {
        // validar campos vazios
        if (id <= 0)
        {
            return Problem("id must be higher than 0");
        }

        try
        {
            var result = _unitOfWork.accountRepository.GetAccounts(id);

            if (result.Result == null || result.Result.Count == 0) return NotFound();

            return Ok(result.Result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }


    [HttpGet]
    [Route("{id}")]
    /// <summary>
    /// Get X account
    /// </summary>
    public IActionResult Accounts(int id, string permission) // remover ambos os params por um token
    {
        return NotFound();
    }
}
