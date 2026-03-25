using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Documentations.Update;

public class UpdateProjectDocumentationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldUpdateDocumentation_WhenDocumentationExists()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Documentation documentation = new(project.Id, "Getting Started", "# Atlas", 1, DocumentationKind.Note, DocumentationStatus.Draft);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation,
    };
    UpdateProjectDocumentationCommandHandler useCase = new(documentationRepository, projectRepository);
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Published);

    Documentation result = await useCase.Execute("proyecto-atlas", "getting-started", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal("quick-start", result.Slug);
    Assert.Equal(input.ContentMarkdown, result.ContentMarkdown);
    Assert.Equal(input.SortOrder, result.SortOrder);
    Assert.Equal(DocumentationKind.Note, result.Kind);
    Assert.Equal(input.Status, result.Status);
    Assert.Same(documentation, result);
    Assert.Same(documentation, documentationRepository.UpdatedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    UpdateProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Draft);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
        useCase.Execute("missing-project", "getting-started", input));
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationNotFoundException_WhenDocumentationDoesNotExist()
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
    UpdateProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Draft);

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() =>
        useCase.Execute("proyecto-atlas", "missing-doc", input));
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationTitleConventionException_WhenDecisionTitleIsInvalid()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Documentation documentation = new(
        project.Id,
        "ADR-001 Architecture",
        "# Atlas",
        1,
        DocumentationKind.Decision,
        DocumentationStatus.Draft);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation,
    };
    UpdateProjectDocumentationCommandHandler useCase = new(documentationRepository, projectRepository);
    UpdateProjectDocumentationCommand input = new(
        "Architecture without ADR prefix",
        "## Updated",
        3,
        DocumentationStatus.Published);

    InvalidDocumentationTitleConventionException exception =
        await Assert.ThrowsAsync<InvalidDocumentationTitleConventionException>(() =>
            useCase.Execute("proyecto-atlas", "adr-001-architecture", input));

    Assert.Contains("ADR-XXX", exception.Message, StringComparison.Ordinal);
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
    UpdateProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Draft);

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!, input));
  }
}
