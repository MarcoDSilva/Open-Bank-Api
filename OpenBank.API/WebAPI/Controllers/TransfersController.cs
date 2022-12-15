using Microsoft.AspNetCore.Mvc;
using OpenBank.API.DTO;



namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{

    [HttpPost]
    public IActionResult Transfers(TransferRequest transferRequest)
    {
        return Ok();
    }

}
