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
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationCommand input = new(
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
  public async Task Execute_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
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
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationCommand input = new(
        "Guía API: sección / inicial",
        "# Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    Assert.Equal("guia-api-seccion-inicial", result.Slug);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationTitleConventionException_WhenDecisionTitleIsInvalid()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(
        "Decision without ADR prefix",
        "# Atlas",
        1,
        DocumentationKind.Decision,
        DocumentationStatus.Draft);

    InvalidDocumentationTitleConventionException exception =
        await Assert.ThrowsAsync<InvalidDocumentationTitleConventionException>(() =>
            createDocumentationUseCase.Execute("proyecto-atlas", input));

    Assert.Contains("ADR-XXX", exception.Message, StringComparison.Ordinal);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    CreateProjectDocumentationCommand input = new(
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
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(title!, contentMarkdown!, 1, DocumentationKind.Note, DocumentationStatus.Draft);

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        createDocumentationUseCase.Execute(projectSlug!, input));
  }
}
