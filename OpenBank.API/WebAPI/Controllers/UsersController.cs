using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.DTO;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // call to verify the connection with the db
    [HttpGet]
    public IActionResult Users()
    {
        var users = _unitOfWork.userRepository.GetAllUsers().ToList();

        if ((int)users.Count <= 0)
            return NotFound("Error 404: There are no users registered");

        return Ok(users);
    }

    [HttpPost]
    public IActionResult Users(CreateUserRequest createUser)
    {
        // validar campos vazios

        bool usernameAvailable = _unitOfWork.userRepository.IsUsernameAvailable(createUser.Username);
        if (!usernameAvailable)
        {
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");
        }

        try
        {
            var result = _unitOfWork.userRepository.CreateUser(createUser);
            return Ok(result.Result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUserRequest loginRequest)
    {
        try
        {
            bool loginValid = _unitOfWork.userRepository.IsLoginValid(loginRequest);

            if (!loginValid)
            {
                return Unauthorized("Login or username incorrect!");
            }

            return Ok(loginRequest.UserName);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }


    }
}
