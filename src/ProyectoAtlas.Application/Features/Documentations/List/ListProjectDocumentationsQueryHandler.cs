using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.List;

public class ListProjectDocumentationsQueryHandler(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task<ListProjectDocumentationsResponse> Execute(string projectSlug, ListProjectDocumentationsQuery input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    (int page, int pageSize, string? query, DocumentationKind? kind, DocumentationStatus? status, DocumentationArea? area, string? tag) = input;

    (IEnumerable<Documentation>? documentations, int totalCount) = await documentationRepository.GetPagedList(
      project.Id,
      page,
      pageSize,
      query,
      kind,
      status,
      area,
      tag,
      cancellationToken);

    List<Documentation> items = [.. documentations];
    int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    return new ListProjectDocumentationsResponse(items, page, pageSize, totalPages, totalCount);
  }
}
