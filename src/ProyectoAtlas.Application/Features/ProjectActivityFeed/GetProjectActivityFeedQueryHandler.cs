using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectActivityFeed;

public class GetProjectActivityFeedQueryHandler(
    IProjectActivityFeedRepository projectActivityFeedRepository,
    IProjectRepository projectRepository)
{
  public async Task<GetProjectActivityFeedResponse> Execute(
      string projectSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    IReadOnlyCollection<ProjectActivityFeedItem> items = await projectActivityFeedRepository.GetItems(
        project.Id,
        cancellationToken);

    return new GetProjectActivityFeedResponse(items);
  }
}
