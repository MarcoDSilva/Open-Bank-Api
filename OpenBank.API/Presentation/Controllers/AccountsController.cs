using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;
using AutoMapper;
using OpenBank.Api.Shared;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountBusinessRules _accountBusiness;
    private readonly ITransferBusinessRules _transferBusiness;
    private readonly IMapper _mapper;

    public AccountsController(IUnitOfWork unitOfWork, IAccountBusinessRules accountBusinessRules, ITransferBusinessRules transferBusinessRules, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _accountBusiness = accountBusinessRules;
        _transferBusiness = transferBusinessRules;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
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
            Account account = new Account()
            {
                Balance = accountRequest.Amount,
                Created_at = DateTime.UtcNow,
                Currency = accountRequest.Currency,
                UserId = userId
            };

            (bool, Account) result = await _accountBusiness.CreateAccount(userId, account);

            if (!result.Item1)
                return Problem();

            var response = _mapper.Map<Account, AccountResponse>(result.Item2);

            return Ok(response);
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
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        try
        {
            List<Account> accounts = await _accountBusiness.GetAccounts(userId);

            if (accounts == null || accounts.Count == 0)
                return NotFound("This user has no accounts.");

            List<AccountResponse> accountResponseDTO = new List<AccountResponse>();

            accounts.ForEach(acc => accountResponseDTO.Add(_mapper.Map<Account, AccountResponse>(acc)));

            return Ok(accountResponseDTO);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Accounts with the message: {e.Message}");
            return Problem(e.Message);
        }
    }


    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(AccountMovement), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

    /// <summary>
    /// Get the account and movements associated with the id
    /// </summary>
    public async Task<IActionResult> Accounts([FromRoute] int id)
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        if (userId <= 0)
            return Unauthorized(AccountDescriptions.NotLoggedIn);

        // validar campos vazios
        if (id <= 0)
            return BadRequest("Account id must be higher than 0");

        Account? account = await _accountBusiness.GetAccountById(id, userId);

        if (account is null)
            return NotFound(AccountDescriptions.AccountNonExistant);

        try
        {
            List<Transfer> movements = await _transferBusiness.GetAccountMovementsAsync(account.Id);

            List<MovementResponse> movementsDTO = new List<MovementResponse>();
            movements.ForEach(mov => movementsDTO.Add(_mapper.Map<Transfer, MovementResponse>(mov)));

            AccountMovement accountWithMovements = new AccountMovement()
            {
                Account = _mapper.Map<Account, AccountResponse>(account),
                Movements = movementsDTO
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
