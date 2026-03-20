namespace ProyectoAtlas.Application.Tests;

public class HealthCheckUseCaseTests
{
    [Fact]
    public void Execute_ShouldReturnOk()
    {

        var healthCheckUseCase = new HealthCheckUseCase();


        var result = healthCheckUseCase.Execute();

        // Assert
        Assert.Equal("ok", result);
    }
}
