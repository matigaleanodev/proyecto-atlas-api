using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Features.Delete;

public class DeleteProjectFeatureCommandHandler(
    IFeatureRepository featureRepository,
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IMilestoneFeatureLinkRepository milestoneFeatureLinkRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Domain.Features.Feature feature = await featureRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, slug);

    IReadOnlyCollection<Domain.Features.FeatureDocumentationLink> documentationLinks =
        await featureDocumentationLinkRepository.GetByFeatureId(feature.Id, cancellationToken);

    if (documentationLinks.Count > 0)
    {
      throw new FeatureDeleteBlockedException(projectSlug, slug, "it is still linked to one or more documentation items");
    }

    IReadOnlyCollection<Domain.Milestones.MilestoneFeatureLink> milestoneLinks =
        await milestoneFeatureLinkRepository.GetByFeatureId(feature.Id, cancellationToken);

    if (milestoneLinks.Count > 0)
    {
      throw new FeatureDeleteBlockedException(projectSlug, slug, "it is still linked to one or more milestones");
    }

    await featureRepository.Delete(feature, cancellationToken);
  }
}
