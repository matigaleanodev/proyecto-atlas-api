using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class UpdateProjectUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldUpdateProvidedFieldsAndReturnProject()
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
    UpdateProjectUseCase useCase = new UpdateProjectUseCase(projectRepository);
    UpdateProjectInput input = new UpdateProjectInput(
        "Proyecto Atlas API",
        "Updated backend for project documentation",
        "https://github.com/matigaleanodev/proyecto-atlas-api-updated",
        "#0F172A");

    Project result = await useCase.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Description, result.Description);
    Assert.Equal(input.RepositoryUrl, result.RepositoryUrl);
    Assert.Equal(input.Color, result.Color);
    Assert.Equal("proyecto-atlas-api", result.Slug);
    Assert.Same(result, projectRepository.UpdatedProject);
  }

  [Fact]
  public async Task Execute_ShouldRecalculateSlug_WhenTitleChanges()
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
    UpdateProjectUseCase useCase = new UpdateProjectUseCase(projectRepository);
    UpdateProjectInput input = new UpdateProjectInput("Atlas Platform", null, null, null);

    Project result = await useCase.Execute("proyecto-atlas", input);

    Assert.Equal("atlas-platform", result.Slug);
  }

  [Fact]
  public async Task Execute_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
  {
    UpdateProjectUseCase useCase = new UpdateProjectUseCase(new FakeProjectRepository());
    UpdateProjectInput input = new UpdateProjectInput("Atlas Platform", null, null, null);

    await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.Execute("missing-project", input));
  }
}
