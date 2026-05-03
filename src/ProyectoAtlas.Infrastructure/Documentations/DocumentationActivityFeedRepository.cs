using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Application.Features.DocumentationActivityFeed;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Documentations;

public class DocumentationActivityFeedRepository(ProyectoAtlasDbContext dbContext) : IDocumentationActivityFeedRepository
{
  public async Task<IReadOnlyCollection<DocumentationActivityFeedItem>> GetItems(
      Guid documentationId,
      CancellationToken cancellationToken = default)
  {
    List<DocumentationActivityFeedItem> auditItems = await dbContext.AuditEvents
        .Where(item => item.DocumentationId == documentationId)
        .Select(item => new DocumentationActivityFeedItem(
            DocumentationActivityFeedItemType.AuditEvent,
            item.OccurredAtUtc,
            item.Action,
            item.EntitySlug,
            item.EntityTitle,
            null,
            null,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<DocumentationActivityFeedItem> versionItems = await dbContext.DocumentationVersions
        .Where(item => item.DocumentationId == documentationId)
        .Select(item => new DocumentationActivityFeedItem(
            DocumentationActivityFeedItemType.VersionCreated,
            item.CreatedAtUtc,
            null,
            null,
            item.Title,
            item.VersionNumber,
            item.Status,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<DocumentationActivityFeedItem> outgoingRelationItems = await (
        from relation in dbContext.DocumentationRelations
        join relatedDocumentation in dbContext.Documentations on relation.TargetDocumentationId equals relatedDocumentation.Id
        where relation.SourceDocumentationId == documentationId
        select new DocumentationActivityFeedItem(
            DocumentationActivityFeedItemType.RelationCreated,
            relation.CreatedAtUtc,
            null,
            null,
            null,
            null,
            null,
            relation.Kind,
            DocumentationActivityRelationDirection.Outgoing,
            relatedDocumentation.Slug,
            relatedDocumentation.Title))
        .ToListAsync(cancellationToken);

    List<DocumentationActivityFeedItem> incomingRelationItems = await (
        from relation in dbContext.DocumentationRelations
        join relatedDocumentation in dbContext.Documentations on relation.SourceDocumentationId equals relatedDocumentation.Id
        where relation.TargetDocumentationId == documentationId
        select new DocumentationActivityFeedItem(
            DocumentationActivityFeedItemType.RelationCreated,
            relation.CreatedAtUtc,
            null,
            null,
            null,
            null,
            null,
            relation.Kind,
            DocumentationActivityRelationDirection.Incoming,
            relatedDocumentation.Slug,
            relatedDocumentation.Title))
        .ToListAsync(cancellationToken);

    return auditItems
        .Concat(versionItems)
        .Concat(outgoingRelationItems)
        .Concat(incomingRelationItems)
        .OrderByDescending(item => item.OccurredAtUtc)
        .ToList();
  }
}
