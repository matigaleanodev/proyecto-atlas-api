using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.Delete;

public class DeleteFeatureDocumentationLinkCommandHandler(
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      string featureSlug,
      Guid linkId,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(featureSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Feature feature = await featureRepository.GetBySlug(project.Id, featureSlug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, featureSlug);

    FeatureDocumentationLink link = await featureDocumentationLinkRepository.GetById(feature.Id, linkId, cancellationToken)
        ?? throw new FeatureDocumentationLinkNotFoundException(linkId);

    await featureDocumentationLinkRepository.Delete(link, cancellationToken);
  }
}
