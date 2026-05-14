using ProyectoAtlas.Application.Features.Documentations.List;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.List;

public class ListProjectReleaseNotesQueryHandler(ListProjectDocumentationsQueryHandler listProjectDocumentationsQueryHandler)
{
  public Task<ListProjectDocumentationsResponse> Execute(
      string projectSlug,
      ListProjectReleaseNotesQuery input,
      CancellationToken cancellationToken = default)
  {
    ListProjectDocumentationsQuery query = new(
        input.Page,
        input.PageSize,
        input.Query,
        DocumentationKind.ReleaseNotes,
        input.Status,
        input.Area,
        input.Tag);

    return listProjectDocumentationsQueryHandler.Execute(projectSlug, query, cancellationToken);
  }
}
