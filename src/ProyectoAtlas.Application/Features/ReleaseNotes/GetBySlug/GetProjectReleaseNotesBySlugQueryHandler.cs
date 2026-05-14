using ProyectoAtlas.Application.Features.Documentations.GetBySlug;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.GetBySlug;

public class GetProjectReleaseNotesBySlugQueryHandler(GetProjectDocumentationBySlugQueryHandler getProjectDocumentationBySlugQueryHandler)
{
  public async Task<Documentation> Execute(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await getProjectDocumentationBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);

    if (documentation.Kind != DocumentationKind.ReleaseNotes)
    {
      throw new DocumentationNotFoundException(projectSlug, slug);
    }

    return documentation;
  }
}
