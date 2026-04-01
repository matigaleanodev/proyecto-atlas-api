using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Application.Features.Features.Common;

namespace ProyectoAtlas.Application.Features.Features.GetBySlug;

public class GetProjectFeatureBySlugQueryHandler(
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task<Feature> Execute(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    return await featureRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, slug);
  }
}
