using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Documentations.Delete;

public class DeleteProjectDocumentationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteDocumentation_WhenDocumentationExists()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Documentation documentation = new(
        project.Id,
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation,
    };
    DeleteProjectDocumentationCommandHandler useCase = new(documentationRepository, projectRepository);

    await useCase.Execute("proyecto-atlas", "getting-started");

    Assert.Same(documentation, documentationRepository.DeletedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    DeleteProjectDocumentationCommandHandler useCase = new(
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
    DeleteProjectDocumentationCommandHandler useCase = new(
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
    DeleteProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!));
  }
}
