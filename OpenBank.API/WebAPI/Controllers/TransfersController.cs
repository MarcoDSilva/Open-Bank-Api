using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBank.API.DTO;
using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransfersController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;

    public TransfersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> Transfers(TransferRequest transferRequest)
    {
        // if (login != true)
        //     return Unauthorized();

        // if (permission != true)
        //    return Forbid();        

        try
        {
            var result = await _unitOfWork.transferRepository.TransferRequestAsync(transferRequest);

            switch (result.Item1)
            {
                case Infrastructure.Enum.StatusCode.Sucess:
                    return Ok(result.Item2);
                case Infrastructure.Enum.StatusCode.BadRequest:
                    return BadRequest(result.Item2);
                case Infrastructure.Enum.StatusCode.NotFound:
                    return NotFound(result.Item2);
                default:
                    return Ok(result.Item2);
            }
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

}
