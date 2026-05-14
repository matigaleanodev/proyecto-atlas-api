using ProyectoAtlas.Application.Features.ReleaseNotes.GetBySlug;
using ProyectoAtlas.Application.Features.ReleaseNotes.Update;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ReleaseNotes.Update;

public class UpdateProjectReleaseNotesCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldUpdateReleaseNotes_WhenKindMatches()
  {
    Project project = CreateProject();
    Documentation documentation = new(
        project.Id,
        "Release 1.0.0",
        "## Highlights",
        1,
        DocumentationKind.ReleaseNotes,
        DocumentationStatus.Draft,
        DocumentationArea.Product);
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    GetProjectDocumentationBySlugQueryHandler getDocumentationHandler = new(documentationRepository, projectRepository);
    GetProjectReleaseNotesBySlugQueryHandler getReleaseNotesHandler = new(getDocumentationHandler);
    UpdateProjectDocumentationCommandHandler updateDocumentationHandler = new(
        documentationRepository,
        new FakeAuditEventRepository(),
        new FakeDocumentationVersionRepository(),
        projectRepository);
    UpdateProjectReleaseNotesCommandHandler handler = new(getReleaseNotesHandler, updateDocumentationHandler);

    Documentation updated = await handler.Execute(
        "proyecto-atlas",
        documentation.Slug,
        new UpdateProjectReleaseNotesCommand("Release 1.0.1", "## Patch", 2, DocumentationStatus.Published));

    Assert.Equal("Release 1.0.1", updated.Title);
    Assert.Equal(DocumentationKind.ReleaseNotes, updated.Kind);
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
