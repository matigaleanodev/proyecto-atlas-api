using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class ListProjectDocumentationsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnPagedDocumentations()
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
      PagedDocumentations =
      [
        new Documentation(project.Id, "Getting Started", "# Atlas", 1, DocumentationKind.Note, DocumentationStatus.Draft),
        new Documentation(project.Id, "ADR-001 Architecture", "## Layers", 2, DocumentationKind.Note, DocumentationStatus.Published),
      ],
      PagedTotalCount = 2,
    };
    ListProjectDocumentationsQueryHandler useCase = new(documentationRepository, projectRepository);
    ListProjectDocumentationsQuery input = new(1, 10, "atlas");

    ListProjectDocumentationsResponse result = await useCase.Execute("proyecto-atlas", input);

    Assert.Equal(1, result.Page);
    Assert.Equal(10, result.PageSize);
    Assert.Equal(2, result.TotalItems);
    Assert.Equal(1, result.TotalPages);
    Assert.Equal(2, result.Items.Count);
    Assert.Equal(DocumentationKind.Note, result.Items.First().Kind);
    Assert.Equal(DocumentationStatus.Draft, result.Items.First().Status);
  }

  [Fact]
  public async Task Execute_ShouldPassArgumentsToRepository()
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
    FakeDocumentationRepository documentationRepository = new();
    ListProjectDocumentationsQueryHandler useCase = new(documentationRepository, projectRepository);
    ListProjectDocumentationsQuery input = new(
        2,
        5,
        "docs",
        DocumentationKind.Decision,
        DocumentationStatus.Published);

    await useCase.Execute("proyecto-atlas", input);

    Assert.Equal(project.Id, documentationRepository.ReceivedProjectId);
    Assert.Equal(2, documentationRepository.ReceivedPage);
    Assert.Equal(5, documentationRepository.ReceivedPageSize);
    Assert.Equal("docs", documentationRepository.ReceivedQuery);
    Assert.Equal(DocumentationKind.Decision, documentationRepository.ReceivedKind);
    Assert.Equal(DocumentationStatus.Published, documentationRepository.ReceivedStatus);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    ListProjectDocumentationsQueryHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    ListProjectDocumentationsQuery input = new();

    await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
        useCase.Execute("missing-project", input));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenProjectSlugIsInvalid(string? projectSlug)
  {
    ListProjectDocumentationsQueryHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    ListProjectDocumentationsQuery input = new();

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, input));
  }
}
