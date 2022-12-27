using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class LoginController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LoginController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("users/login")]
    [ProducesResponseType(typeof(IEnumerable<LoginUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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

            if (userId > 0)
            {
                // creating token
                LoginUserResponse loginUserResponse = await _unitOfWork.tokenHandler.CreateTokenAsync(loginRequest, userId);

                if (string.IsNullOrWhiteSpace(loginUserResponse?.AcessToken))
                    return Problem("Could not login due to issues with the server");

                return Ok(loginUserResponse);
            }

            return Unauthorized("Login or username incorrect!");
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Login with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}