namespace ProyectoAtlas.Application.Features.DocumentationActivityFeed;

public interface IDocumentationActivityFeedRepository
{
  Task<IReadOnlyCollection<DocumentationActivityFeedItem>> GetItems(
      Guid documentationId,
      CancellationToken cancellationToken = default);
}
