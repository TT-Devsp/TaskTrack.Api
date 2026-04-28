using Microsoft.AspNetCore.Mvc;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoint de verificacao de saude da API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>Retorna status basico do servico.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "TaskTrack.Api",
            timestampUtc = DateTime.UtcNow
        });
    }
}
