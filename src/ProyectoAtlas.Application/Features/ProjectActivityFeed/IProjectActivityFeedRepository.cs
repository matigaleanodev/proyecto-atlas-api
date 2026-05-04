namespace ProyectoAtlas.Application.Features.ProjectActivityFeed;

public interface IProjectActivityFeedRepository
{
  Task<IReadOnlyCollection<ProjectActivityFeedItem>> GetItems(Guid projectId, CancellationToken cancellationToken = default);
}
