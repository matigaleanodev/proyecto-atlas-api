using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.ListByDocumentation;

public class ListDocumentationFeatureLinksQueryHandler(
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListDocumentationFeatureLinksResponse> Execute(
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

    IReadOnlyCollection<FeatureDocumentationLink> links = await featureDocumentationLinkRepository.GetByDocumentationId(
        documentation.Id,
        cancellationToken);

    return new ListDocumentationFeatureLinksResponse(links);
  }
}
