using ProyectoAtlas.Application.Features.ReleaseNotes.Delete;
using ProyectoAtlas.Application.Features.ReleaseNotes.GetBySlug;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ReleaseNotes.Delete;

public class DeleteProjectReleaseNotesCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteReleaseNotes_WhenKindMatches()
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
    DeleteProjectDocumentationCommandHandler deleteDocumentationHandler = new(
        documentationRepository,
        new FakeDocumentationRelationRepository(),
        new FakeFeatureDocumentationLinkRepository(),
        new FakeAuditEventRepository(),
        projectRepository);
    DeleteProjectReleaseNotesCommandHandler handler = new(getReleaseNotesHandler, deleteDocumentationHandler);

    await handler.Execute("proyecto-atlas", documentation.Slug);

    Assert.Same(documentation, documentationRepository.DeletedDocumentation);
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
