using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Documentations.GetBySlug;

public class GetProjectDocumentationBySlugQueryHandlerTests
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
      DocumentationBySlug = new Documentation(
          project.Id,
          "Getting Started",
          "# Atlas",
          1,
          DocumentationKind.Note,
          DocumentationStatus.Draft,
          DocumentationArea.Backend),
    };
    GetProjectDocumentationBySlugQueryHandler useCase = new(documentationRepository, projectRepository);

    Documentation result = await useCase.Execute("proyecto-atlas", "getting-started");

    Assert.Equal("Getting Started", result.Title);
    Assert.Equal(DocumentationKind.Note, result.Kind);
    Assert.Equal(DocumentationStatus.Draft, result.Status);
    Assert.Equal(DocumentationArea.Backend, result.Area);
    Assert.Equal(project.Id, documentationRepository.ReceivedProjectId);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    GetProjectDocumentationBySlugQueryHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
        useCase.Execute("missing-project", "getting-started"));
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
    GetProjectDocumentationBySlugQueryHandler useCase = new(
        new FakeDocumentationRepository(),
        projectRepository);

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() =>
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
    GetProjectDocumentationBySlugQueryHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!));
  }
}
