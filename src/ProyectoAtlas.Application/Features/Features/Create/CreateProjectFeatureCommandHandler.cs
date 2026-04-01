using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Features.Create;

public class CreateProjectFeatureCommandHandler(
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task<Feature> Execute(
      string projectSlug,
      CreateProjectFeatureCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Summary);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Feature feature = new(project.Id, input.Title, input.Summary, input.Status);

    await featureRepository.Add(feature, cancellationToken);

    return feature;
  }
}
