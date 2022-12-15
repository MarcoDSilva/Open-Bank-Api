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

    /// <summary>
    /// Get all the accounts
    /// </summary>
    [HttpGet]
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
    public IActionResult Accounts(int idUser, CreateAccountRequest accountRequest)
    {
        // validar campos vazios
        if (idUser <= 0)
        {
            return Problem("ID tem de ser um número válido acima de 0");
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
    [Route("{id}")]
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
            var result = _unitOfWork.accountRepository.GetAccountById(id);

            if (result.Result == null) return NotFound();

            return Ok(result.Result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}
