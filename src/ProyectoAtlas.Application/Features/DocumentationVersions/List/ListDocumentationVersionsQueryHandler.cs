using ProyectoAtlas.Application.Features.DocumentationVersions.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationVersions.List;

public class ListDocumentationVersionsQueryHandler(
    IDocumentationVersionRepository documentationVersionRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListDocumentationVersionsResponse> Execute(
      string projectSlug,
      string documentationSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(documentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, documentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, documentationSlug);

    IReadOnlyCollection<DocumentationVersion> versions = await documentationVersionRepository.GetList(
        documentation.Id,
        cancellationToken);

    return new ListDocumentationVersionsResponse(versions);
  }
}
