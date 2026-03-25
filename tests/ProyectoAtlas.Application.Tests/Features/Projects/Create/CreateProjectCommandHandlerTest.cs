using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class CreateProjectCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnProject()
  {
    FakeProjectRepository projectRepository = new FakeProjectRepository();
    CreateProjectCommandHandler createProjectUseCase = new CreateProjectCommandHandler(projectRepository);
    CreateProjectCommand input = new CreateProjectCommand(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");

    Project result = await createProjectUseCase.Execute(input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Description, result.Description);
    Assert.Equal(input.RepositoryUrl, result.RepositoryUrl);
    Assert.Equal(input.Color, result.Color);
    Assert.NotEqual(Guid.Empty, result.Id);
    Assert.Same(result, projectRepository.AddedProject);
  }

  [Theory]
  [InlineData(null, "description", "https://github.com/example/atlas", "#1E293B")]
  [InlineData("", "description", "https://github.com/example/atlas", "#1E293B")]
  [InlineData("   ", "description", "https://github.com/example/atlas", "#1E293B")]
  [InlineData("Atlas", null, "https://github.com/example/atlas", "#1E293B")]
  [InlineData("Atlas", "", "https://github.com/example/atlas", "#1E293B")]
  [InlineData("Atlas", "   ", "https://github.com/example/atlas", "#1E293B")]
  [InlineData("Atlas", "description", null, "#1E293B")]
  [InlineData("Atlas", "description", "", "#1E293B")]
  [InlineData("Atlas", "description", "   ", "#1E293B")]
  [InlineData("Atlas", "description", "https://github.com/example/atlas", null)]
  [InlineData("Atlas", "description", "https://github.com/example/atlas", "")]
  [InlineData("Atlas", "description", "https://github.com/example/atlas", "   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenInputContainsNullOrWhitespace(
      string? title,
      string? description,
      string? repositoryUrl,
      string? color)
  {
    CreateProjectCommandHandler createProjectUseCase = new CreateProjectCommandHandler(new FakeProjectRepository());
    CreateProjectCommand input = new CreateProjectCommand(title!, description!, repositoryUrl!, color!);

    await Assert.ThrowsAnyAsync<ArgumentException>(() => createProjectUseCase.Execute(input));
  }
}
