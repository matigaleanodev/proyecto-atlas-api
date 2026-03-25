using System.Diagnostics.CodeAnalysis;

namespace ProyectoAtlas.Application.Features.Health.Check;

public class HealthCheckQueryHandler
{
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string Execute()
  {
    return "ok";
  }
}
