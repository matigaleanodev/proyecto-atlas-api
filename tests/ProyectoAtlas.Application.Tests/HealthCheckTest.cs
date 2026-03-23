namespace ProyectoAtlas.Application.Tests;

public class HealthCheckUseCaseTests
{
  [Fact]
  public void Execute_ShouldReturnOk()
  {

    HealthCheckUseCase healthCheckUseCase = new HealthCheckUseCase();


    string result = healthCheckUseCase.Execute();

    // Assert
    Assert.Equal("ok", result);
  }
}
