using Microsoft.AspNetCore.Mvc;
namespace ProyectoAtlas.Api.Features.Health;

[ApiController]
[Route("health")]
public class HealthController(HealthCheckQueryHandler healthCheckQueryHandler) : ControllerBase
{
  [HttpGet]
  public IActionResult Get()
  {
    string status = healthCheckQueryHandler.Execute();

    return Ok(new { status });
  }
}
