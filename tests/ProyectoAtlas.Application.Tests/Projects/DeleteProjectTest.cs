using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class DeleteProjectUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldDeleteProject_WhenSlugExists()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    DeleteProjectUseCase useCase = new DeleteProjectUseCase(projectRepository);

    await useCase.Execute("proyecto-atlas");

    Assert.Same(existingProject, projectRepository.DeletedProject);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    DeleteProjectUseCase useCase = new DeleteProjectUseCase(new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => useCase.Execute("missing-project"));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenSlugIsInvalid(string? slug)
  {
    DeleteProjectUseCase useCase = new DeleteProjectUseCase(new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() => useCase.Execute(slug!));
  }
}
