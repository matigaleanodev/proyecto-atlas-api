namespace ProyectoAtlas.Application.Tests;

public class HealthCheckQueryHandlerTests
{
  [Fact]
  public void Execute_ShouldReturnOk()
  {

    HealthCheckQueryHandler healthCheckUseCase = new HealthCheckQueryHandler();


    string result = healthCheckUseCase.Execute();

    // Assert
    Assert.Equal("ok", result);
  }
}
