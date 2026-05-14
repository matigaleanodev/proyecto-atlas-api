using ProyectoAtlas.Application.Features.Documentations.Update;
using ProyectoAtlas.Application.Features.ReleaseNotes.GetBySlug;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.Update;

public class UpdateProjectReleaseNotesCommandHandler(
    GetProjectReleaseNotesBySlugQueryHandler getProjectReleaseNotesBySlugQueryHandler,
    UpdateProjectDocumentationCommandHandler updateProjectDocumentationCommandHandler)
{
  public async Task<Documentation> Execute(
      string projectSlug,
      string slug,
      UpdateProjectReleaseNotesCommand input,
      CancellationToken cancellationToken = default)
  {
    await getProjectReleaseNotesBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);

    UpdateProjectDocumentationCommand command = new(
        input.Title,
        input.ContentMarkdown,
        input.SortOrder,
        input.Status,
        null,
        input.Tags);

    return await updateProjectDocumentationCommandHandler.Execute(projectSlug, slug, command, cancellationToken);
  }
}
