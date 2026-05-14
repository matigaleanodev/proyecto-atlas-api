using ProyectoAtlas.Application.Features.ProjectActivityFeed;

namespace ProyectoAtlas.Application.Tests.Features.ProjectActivityFeed;

internal sealed class FakeProjectActivityFeedRepository : IProjectActivityFeedRepository
{
  public Guid ReceivedProjectId { get; private set; }
  public IReadOnlyCollection<ProjectActivityFeedItem> Items { get; set; } = [];

  public Task<IReadOnlyCollection<ProjectActivityFeedItem>> GetItems(Guid projectId, CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    return Task.FromResult(Items);
  }
}
