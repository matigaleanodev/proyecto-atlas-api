using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.ListByFeature;

public class ListFeatureDocumentationLinksQueryHandler(
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListFeatureDocumentationLinksResponse> Execute(
      string projectSlug,
      string featureSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(featureSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Feature feature = await featureRepository.GetBySlug(project.Id, featureSlug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, featureSlug);

    IReadOnlyCollection<FeatureDocumentationLink> links = await featureDocumentationLinkRepository.GetByFeatureId(
        feature.Id,
        cancellationToken);

    return new ListFeatureDocumentationLinksResponse(links);
  }
}
