using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Projects.GetBySlug;

public class GetProjectBySlugQueryHandlerTests
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
    GetProjectBySlugQueryHandler useCase = new GetProjectBySlugQueryHandler(projectRepository);

    Project result = await useCase.Execute("proyecto-atlas");

    Assert.NotNull(result);
    Assert.Equal(projectRepository.ProjectBySlug!.Title, result.Title);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenSlugDoesNotExist()
  {
    GetProjectBySlugQueryHandler useCase = new GetProjectBySlugQueryHandler(new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => useCase.Execute("missing-project"));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenSlugIsInvalid(string? slug)
  {
    GetProjectBySlugQueryHandler useCase = new GetProjectBySlugQueryHandler(new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() => useCase.Execute(slug!));
  }
}
