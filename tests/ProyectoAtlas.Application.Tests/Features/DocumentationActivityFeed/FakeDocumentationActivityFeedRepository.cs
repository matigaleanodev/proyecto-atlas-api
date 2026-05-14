using ProyectoAtlas.Application.Features.DocumentationActivityFeed;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationActivityFeed;

internal sealed class FakeDocumentationActivityFeedRepository : IDocumentationActivityFeedRepository
{
  public Guid ReceivedDocumentationId { get; private set; }

  public IReadOnlyCollection<DocumentationActivityFeedItem> Items { get; set; } = [];

  public Task<IReadOnlyCollection<DocumentationActivityFeedItem>> GetItems(
      Guid documentationId,
      CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    return Task.FromResult(Items);
  }
}
