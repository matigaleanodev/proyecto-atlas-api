using ProyectoAtlas.Application.Features.ReleaseNotes.List;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ReleaseNotes.List;

public class ListProjectReleaseNotesQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldForceReleaseNotesKindFilter()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeDocumentationRepository documentationRepository = new()
    {
      PagedDocumentations =
      [
        new Documentation(project.Id, "Release 1.0.0", "## Highlights", 1, DocumentationKind.ReleaseNotes, DocumentationStatus.Published, DocumentationArea.Product)
      ],
      PagedTotalCount = 1
    };
    ListProjectDocumentationsQueryHandler documentationHandler = new(
        documentationRepository,
        new FakeProjectRepository { ProjectBySlug = project });
    ListProjectReleaseNotesQueryHandler handler = new(documentationHandler);

    await handler.Execute("proyecto-atlas", new ListProjectReleaseNotesQuery(Query: "1.0.0"));

    Assert.Equal(DocumentationKind.ReleaseNotes, documentationRepository.ReceivedKind);
    Assert.Equal("1.0.0", documentationRepository.ReceivedQuery);
  }
}
