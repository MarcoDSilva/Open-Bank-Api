using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Data;
using OpenBank.API.Models.Entities;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountsController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    /// <summary>
    /// Get all the accounts
    /// </summary>
    [HttpGet]
    [Route("accounts")]
    public IActionResult Accounts()
    {
        return Ok();
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
    [Route("accounts")]
    public IActionResult Accounts(int idUser, CreateAccountRequest accountRequest)
    {
        // validar campos vazios
        if (idUser <= 0)
        {
            return Problem("ID tem de ser um número válido acima de 0");
        }

        try
        {
            var result = _accountRepository.CreateAccount(idUser, accountRequest);
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
    [Route("accounts/{id}")]
    /// <summary>
    /// Get X account
    /// </summary>
    public IActionResult Accounts(int id)
    {
        // validar campos vazios
        if (id <= 0)
        {
            return Problem("id tem de ser um número válido acima de 0");
        }

        try
        {
            var result = _accountRepository.GetAccountById(id);

            if (result.Result == null) return NotFound();

            return Ok(result.Result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}
