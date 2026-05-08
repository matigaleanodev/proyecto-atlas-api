using ProyectoAtlas.Application.Features.ReleaseNotes.GetBySlug;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ReleaseNotes.GetBySlug;

public class GetProjectReleaseNotesBySlugQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnReleaseNotes_WhenKindMatches()
  {
    Project project = CreateProject();
    Documentation documentation = new(
        project.Id,
        "Release 1.0.0",
        "## Highlights",
        1,
        DocumentationKind.ReleaseNotes,
        DocumentationStatus.Published,
        DocumentationArea.Product);
    GetProjectDocumentationBySlugQueryHandler documentationHandler = new(
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeProjectRepository { ProjectBySlug = project });
    GetProjectReleaseNotesBySlugQueryHandler handler = new(documentationHandler);

    Documentation releaseNotes = await handler.Execute("proyecto-atlas", documentation.Slug);

    Assert.Equal(DocumentationKind.ReleaseNotes, releaseNotes.Kind);
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationNotFoundException_WhenSlugBelongsToAnotherKind()
  {
    Project project = CreateProject();
    Documentation documentation = new(
        project.Id,
        "Getting Started",
        "# Intro",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
    GetProjectDocumentationBySlugQueryHandler documentationHandler = new(
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeProjectRepository { ProjectBySlug = project });
    GetProjectReleaseNotesBySlugQueryHandler handler = new(documentationHandler);

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() =>
        handler.Execute("proyecto-atlas", documentation.Slug));
  }

  private static Project CreateProject()
  {
    return new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }
}
