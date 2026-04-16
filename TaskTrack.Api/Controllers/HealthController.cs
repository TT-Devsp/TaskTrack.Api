using Microsoft.AspNetCore.Mvc;

namespace TaskTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
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
