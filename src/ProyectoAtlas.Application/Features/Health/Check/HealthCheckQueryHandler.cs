using System.Diagnostics.CodeAnalysis;

namespace ProyectoAtlas.Application;

public class HealthCheckQueryHandler
{
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string Execute()
  {
    return "ok";
  }
}
