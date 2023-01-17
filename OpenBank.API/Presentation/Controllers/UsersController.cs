using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Services.Interfaces;
using AutoMapper;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userServices;
    private readonly IMapper _mapper;

    public UsersController(IUserService userServices, IMapper mapper)
    {
        _userServices = userServices;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Users()
    {
        var users = _userServices.GetAllUsers();

        if ((int)users.Count() <= 0)
            return Ok("There are no users registered");

        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Users(CreateUserRequest createUser)
    {
        var isUsernameAvailable = await _userServices.IsUsernameAvailableAsync(createUser.Username);

        if (!isUsernameAvailable)
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");

        try
        {
            var result = await _userServices.CreateUserAsync(createUser);
            return Ok(result);
        }
        catch (Exception e)
        {
            //_unitOfWork.loggerHandler.Log(LogLevel.Error, $"Error caught on control Users with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
