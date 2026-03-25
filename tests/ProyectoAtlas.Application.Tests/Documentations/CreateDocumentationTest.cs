using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class CreateDocumentationUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentation()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    FakeDocumentationRepository documentationRepository = new();
    CreateProjectDocumentationUseCase createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationInput input = new(
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.ContentMarkdown, result.ContentMarkdown);
    Assert.Equal(input.SortOrder, result.SortOrder);
    Assert.Equal(input.Kind, result.Kind);
    Assert.Equal(input.Status, result.Status);
    Assert.Equal(projectRepository.ProjectBySlug!.Id, result.ProjectId);
    Assert.NotEqual(Guid.Empty, result.Id);
    Assert.Same(result, documentationRepository.AddedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    CreateProjectDocumentationUseCase createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    CreateProjectDocumentationInput input = new(
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
        createDocumentationUseCase.Execute("missing-project", input));
  }

  [Theory]
  [InlineData(null, "Getting Started", "# Atlas")]
  [InlineData("", "Getting Started", "# Atlas")]
  [InlineData("   ", "Getting Started", "# Atlas")]
  [InlineData("proyecto-atlas", null, "# Atlas")]
  [InlineData("proyecto-atlas", "", "# Atlas")]
  [InlineData("proyecto-atlas", "   ", "# Atlas")]
  [InlineData("proyecto-atlas", "Getting Started", null)]
  [InlineData("proyecto-atlas", "Getting Started", "")]
  [InlineData("proyecto-atlas", "Getting Started", "   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenInputContainsNullOrWhitespace(
      string? projectSlug,
      string? title,
      string? contentMarkdown)
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationUseCase createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationInput input = new(title!, contentMarkdown!, 1, DocumentationKind.Note, DocumentationStatus.Draft);

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        createDocumentationUseCase.Execute(projectSlug!, input));
  }
}
