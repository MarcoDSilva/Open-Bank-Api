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

    [HttpPost]
    [Route("users")]
    public IActionResult Users(CreateUserRequest createUser)
    {
        var result = _userRepository.CreateUser(createUser);
        // método de validação aqui e retornar dependente do resultado
        return Ok(result);        
    }

    [HttpPost]
    [Route("users/login")]
    public IActionResult Login(LoginUserRequest loginRequest)
    {
        return Ok();
    }
}
