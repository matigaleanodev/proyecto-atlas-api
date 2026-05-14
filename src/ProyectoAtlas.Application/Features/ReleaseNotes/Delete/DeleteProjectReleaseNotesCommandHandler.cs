using ProyectoAtlas.Application.Features.Documentations.Delete;
using ProyectoAtlas.Application.Features.ReleaseNotes.GetBySlug;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.Delete;

public class DeleteProjectReleaseNotesCommandHandler(
    GetProjectReleaseNotesBySlugQueryHandler getProjectReleaseNotesBySlugQueryHandler,
    DeleteProjectDocumentationCommandHandler deleteProjectDocumentationCommandHandler)
{
  public async Task Execute(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    await getProjectReleaseNotesBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);
    await deleteProjectDocumentationCommandHandler.Execute(projectSlug, slug, cancellationToken);
  }
}
