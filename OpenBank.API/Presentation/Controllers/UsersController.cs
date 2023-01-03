using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Application.DTO;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;
using AutoMapper;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserBusinessRules _userBusinessRules;
    private readonly IMapper _mapper;

    public UsersController(IUnitOfWork unitOfWork, IUserBusinessRules userBusinessRules, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userBusinessRules = userBusinessRules;
        _mapper = mapper;
    }

    // call to verify the connection with the db
    [HttpGet]
    public IActionResult Users()
    {
        var users = _userBusinessRules.GetAllUsers();

        if ((int)users.Count() <= 0)
            return Ok("There are no users registered");

        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<CreateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Users(CreateUserRequest createUser)
    {
        bool isUsernameAvailable = await _userBusinessRules.IsUsernameAvailableAsync(createUser.Username);

        if (!isUsernameAvailable)
            return BadRequest($"Username {createUser.Username} already in use, please register with a different username.");

        try
        {
            User newUser = new User()
            {
                Email = createUser.Email,
                FullName = createUser.FullName,
                Password = createUser.Password,
                UserName = createUser.Username,
                Created_at = DateTime.UtcNow
            };

            User result = await _userBusinessRules.CreateUserAsync(newUser);            
            CreateUserResponse userDTO = _mapper.Map<User, CreateUserResponse>(result);
            
            return Ok(userDTO);
        }
        catch (Exception e)
        {
            _unitOfWork.loggerHandler.Log(LogLevel.Error, $"Error caught on control Users with the message: {e.Message}");
            return Problem(e.Message);
        }
    }
}
