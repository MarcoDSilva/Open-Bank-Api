using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.Api.Shared;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class LoginController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userBusinessRules;

    public LoginController(IUnitOfWork unitOfWork, IUserService userBusinessRules)
    {
        _unitOfWork = unitOfWork;
        _userBusinessRules = userBusinessRules;
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
            int userId = await _userBusinessRules.GetUserIdAsync(loginRequest.UserName, loginRequest.Password);

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

    [HttpPost("users/logout")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout()
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();


        // int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        // if (userId <= 0)
        //     return Unauthorized(AccountDescriptions.NotLoggedIn);

        return Ok();
    }

    [HttpPost("users/renew")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RenewToken()
    {
        string authToken = HttpContext.Request.Headers["Authorization"].ToString();
        int userId = _unitOfWork.tokenHandler.GetUserIdByToken(authToken);

        // if (userId <= 0)
        //     return Unauthorized(AccountDescriptions.NotLoggedIn);

        // isRefreshTokenValid
        //  generateNewToken()
        //    return Ok(token)
        // return BadRequest(InvalidToken Login again)




        return Ok();
    }
}