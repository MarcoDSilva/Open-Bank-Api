using Microsoft.AspNetCore.Mvc;
using OpenBank.API.Models.Entities;

namespace OpenBank.API.Controllers;

[ApiController]
[Route("api/")]
public class TransfersController : ControllerBase
{

    [HttpPost]
    [Route("transfers")]
    public IActionResult Transfers(TransferRequest transferRequest)
    {
        return Ok();
    }

}
