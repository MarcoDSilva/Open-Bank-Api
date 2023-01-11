using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;
using AutoMapper;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserBusinessRules _userBusinessRules;
    private readonly IMapper _mapper;

    public UsersController(IUnitOfWork unitOfWork, IUserBusinessRules userBusinessRules, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userBusinessRules = userBusinessRules;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Users()
    {
        var users = _userBusinessRules.GetAllUsers();

        if ((int)users.Count() <= 0)
            return Ok("There are no users registered");

        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<CreateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Users(CreateUserRequest createUser)
    {
        var isUsernameAvailable = await _userBusinessRules.IsUsernameAvailableAsync(createUser.Username);

        if (!isUsernameAvailable)
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");

        try
        {
            var result = await _userBusinessRules.CreateUserAsync(createUser);
            return Ok(result);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Error caught on control Users with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
