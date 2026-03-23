using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class GetProjectBySlugUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnProject_WhenSlugExists()
  {
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B")
    };
    GetProjectBySlugUseCase useCase = new GetProjectBySlugUseCase(projectRepository);

    Project result = await useCase.Execute("proyecto-atlas");

    Assert.NotNull(result);
    Assert.Equal(projectRepository.ProjectBySlug!.Title, result.Title);
  }

  [Fact]
  public async Task Execute_ShouldThrowKeyNotFoundException_WhenSlugDoesNotExist()
  {
    GetProjectBySlugUseCase useCase = new GetProjectBySlugUseCase(new FakeProjectRepository());

    await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.Execute("missing-project"));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenSlugIsInvalid(string? slug)
  {
    GetProjectBySlugUseCase useCase = new GetProjectBySlugUseCase(new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() => useCase.Execute(slug!));
  }
}
