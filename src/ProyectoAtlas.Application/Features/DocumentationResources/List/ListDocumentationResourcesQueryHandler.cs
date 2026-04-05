using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationResources.List;

public class ListDocumentationResourcesQueryHandler(
    IDocumentationResourceRepository documentationResourceRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListDocumentationResourcesResponse> Execute(
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

    IReadOnlyCollection<DocumentationResource> resources = await documentationResourceRepository.GetList(
        documentation.Id,
        cancellationToken);

    return new ListDocumentationResourcesResponse(resources);
  }
}
