using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.DTO;
using Microsoft.AspNetCore.Authorization;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHandler _tokenHandler;

    public UsersController(IUnitOfWork unitOfWork, ITokenHandler tokenHandler)
    {
        _unitOfWork = unitOfWork;
        _tokenHandler = tokenHandler;
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
        // validar campos vazios

        bool usernameAvailable = _unitOfWork.userRepository.IsUsernameAvailable(createUser.Username);

        if (!usernameAvailable)
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");

        try
        {
            var result = await _unitOfWork.userRepository.CreateUser(createUser);
            return Ok(result);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("login")]
    ///<summary>
    /// Login request 
    ///</summary>
    public async Task<IActionResult> Login(LoginUserRequest loginRequest)
    {

        if (string.IsNullOrWhiteSpace(loginRequest?.Password) || string.IsNullOrWhiteSpace(loginRequest?.UserName))
            return BadRequest("Username and password cannot be empty fields");

        try
        {
            int userId = _unitOfWork.userRepository.IsLoginValid(loginRequest);

            if (userId <= 0)
                return Unauthorized("Login or username incorrect!");

            // creating token
            LoginUserResponse loginUserResponse = await _tokenHandler.CreateTokenAsync(loginRequest);

            if (string.IsNullOrWhiteSpace(loginUserResponse?.AcessToken))
                return Problem("Could not login due to issues with the server");

            return Ok(loginUserResponse);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }


    }
}
