using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.DTO;
using OpenBank.API.Domain.Entities;

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
    [HttpPost]
    public async Task<IActionResult> Accounts(int idUser, CreateAccountRequest accountRequest)
    {
        // validar campos vazios
        if (idUser <= 0)
        {
            return Problem("ID must be a number higher than 0");
        }

        try
        {
            var result = await _unitOfWork.accountRepository.CreateAccount(idUser, accountRequest);
            return Ok(result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpGet]
    /// <summary>
    /// WARNING: This is the regular Accounts() Controller, which will use the Token to validate the user and then
    /// show all the associated accounts
    /// </summary>
    public async Task<IActionResult> Accounts(int id)
    {
        // validar campos vazios
        if (id <= 0)
        {
            return Problem("id must be higher than 0");
        }

        try
        {
            var result = await _unitOfWork.accountRepository.GetAccounts(id);

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
    /// Get the account and movements
    /// </summary>
    public async Task<IActionResult> Accounts(int id, string permission) // remover ambos os params por um token
    {
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
