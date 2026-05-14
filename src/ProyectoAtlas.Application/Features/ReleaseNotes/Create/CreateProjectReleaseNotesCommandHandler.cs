using ProyectoAtlas.Application.Features.Documentations.Create;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.Create;

public class CreateProjectReleaseNotesCommandHandler(CreateProjectDocumentationCommandHandler createProjectDocumentationCommandHandler)
{
  public Task<Documentation> Execute(
      string projectSlug,
      CreateProjectReleaseNotesCommand input,
      CancellationToken cancellationToken = default)
  {
    CreateProjectDocumentationCommand command = new(
        input.Title,
        input.ContentMarkdown,
        input.SortOrder,
        DocumentationKind.ReleaseNotes,
        input.Status,
        input.Area,
        input.Tags);

    return createProjectDocumentationCommandHandler.Execute(projectSlug, command, cancellationToken);
  }
}
