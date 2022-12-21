using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.DTO;

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
            return Ok("There are no users registered");

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Users(CreateUserRequest createUser)
    {
        bool usernameAvailable = _unitOfWork.userRepository.IsUsernameAvailable(createUser.Username);

        if (!usernameAvailable)
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");

        try
        {
            CreateUserRequest result = await _unitOfWork.userRepository.CreateUser(createUser);
            return Ok(result);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Error caught on control Users with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
