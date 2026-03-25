using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class DeleteProjectDocumentationUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldDeleteDocumentation_WhenDocumentationExists()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Documentation documentation = new(project.Id, "Getting Started", "# Atlas", 1, DocumentationKind.Note);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation,
    };
    DeleteProjectDocumentationUseCase useCase = new(documentationRepository, projectRepository);

    await useCase.Execute("proyecto-atlas", "getting-started");

    Assert.Same(documentation, documentationRepository.DeletedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    DeleteProjectDocumentationUseCase useCase = new(
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
    DeleteProjectDocumentationUseCase useCase = new(
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
    DeleteProjectDocumentationUseCase useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!));
  }
}
