using ProyectoAtlas.Application.Features.ReleaseNotes.Create;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ReleaseNotes.Create;

public class CreateProjectReleaseNotesCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldCreateReleaseNotes_WithForcedKind()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeDocumentationRepository documentationRepository = new();
    CreateProjectDocumentationCommandHandler documentationHandler = new(
        documentationRepository,
        new FakeAuditEventRepository(),
        new FakeProjectRepository { ProjectBySlug = project });
    CreateProjectReleaseNotesCommandHandler handler = new(documentationHandler);
    CreateProjectReleaseNotesCommand command = new(
        "Release 1.0.0",
        "## Highlights",
        1,
        DocumentationStatus.Published,
        DocumentationArea.Product);

    Documentation releaseNotes = await handler.Execute("proyecto-atlas", command);

    Assert.Equal(DocumentationKind.ReleaseNotes, releaseNotes.Kind);
    Assert.Equal(DocumentationKind.ReleaseNotes, documentationRepository.AddedDocumentation!.Kind);
  }
}
