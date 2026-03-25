using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Documentations;

public class ListProjectDocumentationsUseCase(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task<ListProjectDocumentationsOutput> Execute(string projectSlug, ListProjectDocumentationsInput input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    (int page, int pageSize, string? query, DocumentationKind? kind, DocumentationStatus? status) = input;

    (IEnumerable<Documentation>? documentations, int totalCount) = await documentationRepository.GetPagedList(project.Id, page, pageSize, query, kind, status, cancellationToken);
    List<Documentation> items = [.. documentations];
    int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    return new ListProjectDocumentationsOutput(items, page, pageSize, totalPages, totalCount);
  }
}
