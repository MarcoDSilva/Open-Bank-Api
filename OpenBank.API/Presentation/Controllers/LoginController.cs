using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.DTO;
using Microsoft.AspNetCore.Authorization;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class LoginController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHandler _tokenHandler;

    public LoginController(IUnitOfWork unitOfWork, ITokenHandler tokenHandler)
    {
        _unitOfWork = unitOfWork;
        _tokenHandler = tokenHandler;
    }

    [HttpPost("users/login")]
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
                LoginUserResponse loginUserResponse = await _tokenHandler.CreateTokenAsync(loginRequest, userId);

                if (string.IsNullOrWhiteSpace(loginUserResponse?.AcessToken))
                    return Problem("Could not login due to issues with the server");

                return Ok(loginUserResponse);
            }

            return Unauthorized("Login or username incorrect!");
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}