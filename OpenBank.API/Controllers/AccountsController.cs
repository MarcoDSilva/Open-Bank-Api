using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Models.Entities;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class AccountsController : ControllerBase
{

    /// <summary>
    /// Get all the accounts
    /// </summary>
    [HttpGet]
    [Route("accounts")]
    public IActionResult Accounts()
    {
        return Ok();
    }

    /*
        Create account
    */
    [HttpPost]
    [Route("accounts")]
    public IActionResult Accounts(CreateAccountRequest accountRequest)
    {
        return Ok();
    }

    [HttpGet]
    [Route("accounts/{id}")]
    /// <summary>
    /// Get X account
    /// </summary>
    public IActionResult Accounts(int accountId)
    {
        return Ok("asda");
    }
}
