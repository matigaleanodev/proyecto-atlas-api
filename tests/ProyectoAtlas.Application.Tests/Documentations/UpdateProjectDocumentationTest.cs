using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class UpdateProjectDocumentationUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldUpdateDocumentation_WhenDocumentationExists()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Documentation documentation = new(project.Id, "Getting Started", "# Atlas", 1);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation,
    };
    UpdateProjectDocumentationUseCase useCase = new(documentationRepository, projectRepository);
    UpdateProjectDocumentationInput input = new(
        "Quick Start",
        "## Updated",
        3);

    Documentation result = await useCase.Execute("proyecto-atlas", "getting-started", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal("quick-start", result.Slug);
    Assert.Equal(input.ContentMarkdown, result.ContentMarkdown);
    Assert.Equal(input.SortOrder, result.SortOrder);
    Assert.Same(documentation, result);
    Assert.Same(documentation, documentationRepository.UpdatedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
  {
    UpdateProjectDocumentationUseCase useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    UpdateProjectDocumentationInput input = new(
        "Quick Start",
        "## Updated",
        3);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        useCase.Execute("missing-project", "getting-started", input));
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
    UpdateProjectDocumentationUseCase useCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    UpdateProjectDocumentationInput input = new(
        "Quick Start",
        "## Updated",
        3);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        useCase.Execute("proyecto-atlas", "missing-doc", input));
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
    UpdateProjectDocumentationUseCase useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    UpdateProjectDocumentationInput input = new(
        "Quick Start",
        "## Updated",
        3);

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!, input));
  }
}
