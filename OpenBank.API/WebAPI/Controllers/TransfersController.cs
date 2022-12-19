using Microsoft.AspNetCore.Mvc;
using OpenBank.API.DTO;
using OpenBank.API.Infrastructure.Interfaces;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        try
        {
            var result = _unitOfWork.transferRepository.TransferRequest(transferRequest);
            return Ok();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

}
