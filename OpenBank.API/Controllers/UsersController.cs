using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Data;
using OpenBank.API.Models.Entities;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // call to verify the connection with the db
    [HttpGet]
    [Route("users")]
    public IActionResult Users()
    {
        var users = _userRepository.GetAllUsers();
        return Ok(users);
    }

    [HttpPost]
    [Route("users")]
    public IActionResult Users(CreateUserRequest createUser)
    {
        var usernameAvailable = _userRepository.IsUsernameAvailable(createUser.Username);
        if (!usernameAvailable)
        {
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");
        }

        try
        {
            var result = _userRepository.CreateUser(createUser);
            return Ok(result.Result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost]
    [Route("users/login")]
    public IActionResult Login(LoginUserRequest loginRequest)
    {
        return Ok();
    }
}
