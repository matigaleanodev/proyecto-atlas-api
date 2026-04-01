using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Features.Update;

public class UpdateProjectFeatureCommandHandler(
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task<Feature> Execute(
      string projectSlug,
      string slug,
      UpdateProjectFeatureCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Feature feature = await featureRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, slug);

    feature.Update(input.Title, input.Summary, input.Status);

    await featureRepository.Update(feature, cancellationToken);

    return feature;
  }
}
