using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.BusinessRules.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserBusinessRules _userBusinessRules;

    public UsersController(IUnitOfWork unitOfWork, IUserBusinessRules userBusinessRules)
    {
        _unitOfWork = unitOfWork;
        _userBusinessRules = userBusinessRules;
    }

    // call to verify the connection with the db
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
        bool isUsernameAvailable = await _userBusinessRules.IsUsernameAvailable(createUser.Username);

        if (!isUsernameAvailable)
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");

        try
        {
            CreateUserResponse result = await _userBusinessRules.CreateUser(createUser);
            return Ok(result);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Error caught on control Users with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
