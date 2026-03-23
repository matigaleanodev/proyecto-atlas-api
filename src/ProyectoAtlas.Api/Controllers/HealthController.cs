using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application;
namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController(HealthCheckUseCase healthCheckUseCase) : ControllerBase
{
  [HttpGet]
  public IActionResult Get()
  {
    string status = healthCheckUseCase.Execute();

    return Ok(new { status });
  }
}
