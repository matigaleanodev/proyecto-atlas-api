using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class GetProjectDocumentationBySlugUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentation_WhenDocumentationExists()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = new Documentation(project.Id, "Getting Started", "# Atlas", 1),
    };
    GetProjectDocumentationBySlugUseCase useCase = new(documentationRepository, projectRepository);

    Documentation result = await useCase.Execute("proyecto-atlas", "getting-started");

    Assert.Equal("Getting Started", result.Title);
    Assert.Equal(project.Id, documentationRepository.ReceivedProjectId);
  }

  [Fact]
  public async Task Execute_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
  {
    GetProjectDocumentationBySlugUseCase useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        useCase.Execute("missing-project", "getting-started"));
  }

  [Fact]
  public async Task Execute_ShouldThrowKeyNotFoundException_WhenDocumentationDoesNotExist()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    GetProjectDocumentationBySlugUseCase useCase = new(
        new FakeDocumentationRepository(),
        projectRepository);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        useCase.Execute("proyecto-atlas", "missing-doc"));
  }

  [Theory]
  [InlineData(null, "getting-started")]
  [InlineData("", "getting-started")]
  [InlineData("   ", "getting-started")]
  [InlineData("proyecto-atlas", null)]
  [InlineData("proyecto-atlas", "")]
  [InlineData("proyecto-atlas", "   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenSlugInputIsInvalid(
      string? projectSlug,
      string? slug)
  {
    GetProjectDocumentationBySlugUseCase useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!));
  }
}
