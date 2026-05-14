using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.ListByFeature;

public class ListFeatureMilestoneLinksQueryHandler(
    IMilestoneFeatureLinkRepository milestoneFeatureLinkRepository,
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListFeatureMilestoneLinksResponse> Execute(
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

    IReadOnlyCollection<MilestoneFeatureLink> links = await milestoneFeatureLinkRepository.GetByFeatureId(
        feature.Id,
        cancellationToken);

    return new ListFeatureMilestoneLinksResponse(links);
  }
}
