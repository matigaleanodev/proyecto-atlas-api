using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Features.Delete;

public class DeleteProjectFeatureCommandHandler(
    IFeatureRepository featureRepository,
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

    await featureRepository.Delete(feature, cancellationToken);
  }
}
