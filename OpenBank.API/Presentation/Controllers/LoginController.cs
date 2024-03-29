using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Services.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class LoginController : ControllerBase
{
    private readonly IUserService _userServices;
    private readonly ITokenService _tokenServices;

    public LoginController(IUserService userServices, ITokenService tokenServices)
    {
        _userServices = userServices;
        _tokenServices = tokenServices;
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
            int userId = await _userServices.GetUserIdAsync(loginRequest.UserName, loginRequest.Password);

            if (userId > 0)
            {
                // creating token
                LoginUserResponse loginUserResponse = await _tokenServices.CreateTokenAsync(loginRequest, userId.ToString());

                if (string.IsNullOrWhiteSpace(loginUserResponse?.AcessToken))
                    return Problem("Could not login due to issues with the server");

                return Ok(loginUserResponse);
            }

            return Unauthorized("Login or username incorrect!");
        }
        catch (Exception e)
        {
            //_unitOfWork.loggerHandler.Log(LogLevel.Error, $"Exception caught on controller Login with the message: {e.Message}");
            return Problem(e.Message);
        }
    }

    [HttpPost("users/logout")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout()
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authToken)) return BadRequest("Non existent token.");

        bool isRevoked = await _tokenServices.RevokeToken(authToken);
        // invalidate local session when frontend is added here
        return isRevoked ? Ok() : Problem();
    }

    [HttpPost("users/renew")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RenewToken()
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authToken)) return BadRequest("Non existant token");

        try
        {
            var validatedToken = await _tokenServices.ValidateToken(authToken);
            validatedToken.UsedDate = DateTime.UtcNow;

            var refreshedToken = await _tokenServices.RenewTokenAsync(validatedToken);

            if (refreshedToken is null)
                return BadRequest();

            return Ok(refreshedToken);
        }
        catch (Exception e)
        {
            switch (e)
            {
                case ExpiredTokenException:
                    return BadRequest(e.Message);
                case InvalidUserAccessException:
                case IllegalTokenException:
                    return Forbid(e.Message);
                default:
                    return Problem(e.Message);
            }
        }
    }
}