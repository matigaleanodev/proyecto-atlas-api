using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Features.List;

public class ListProjectFeaturesQueryHandler(
    IFeatureRepository featureRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListProjectFeaturesResponse> Execute(
      string projectSlug,
      ListProjectFeaturesQuery input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    if (input.Page < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(input), "Page must be greater than 0.");
    }

    if (input.PageSize < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(input), "Page size must be greater than 0.");
    }

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    (IEnumerable<Feature> features, int totalCount) = await featureRepository.GetPagedList(
        project.Id,
        input.Page,
        input.PageSize,
        input.Query,
        input.Status,
        cancellationToken);

    int totalPages = totalCount == 0
        ? 0
        : (int)Math.Ceiling(totalCount / (double)input.PageSize);

    return new ListProjectFeaturesResponse([.. features], input.Page, input.PageSize, totalPages, totalCount);
  }
}
