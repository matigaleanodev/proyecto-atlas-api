using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Application.Features.ProjectActivityFeed;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Projects;

public class ProjectActivityFeedRepository(ProyectoAtlasDbContext dbContext) : IProjectActivityFeedRepository
{
  public async Task<IReadOnlyCollection<ProjectActivityFeedItem>> GetItems(
      Guid projectId,
      CancellationToken cancellationToken = default)
  {
    List<ProjectActivityFeedItem> auditItems = await dbContext.AuditEvents
        .Where(item => item.ProjectId == projectId)
        .Select(item => new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.AuditEvent,
            item.OccurredAtUtc,
            item.EntitySlug,
            item.EntityTitle,
            item.Action,
            null,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> featureCreatedItems = await dbContext.Features
        .Where(item => item.ProjectId == projectId)
        .Select(item => new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.FeatureCreated,
            item.CreatedAtUtc,
            item.Slug,
            item.Title,
            null,
            null,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> featureUpdatedItems = await dbContext.Features
        .Where(item => item.ProjectId == projectId && item.UpdatedAtUtc > item.CreatedAtUtc)
        .Select(item => new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.FeatureUpdated,
            item.UpdatedAtUtc,
            item.Slug,
            item.Title,
            null,
            null,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> milestoneCreatedItems = await dbContext.Milestones
        .Where(item => item.ProjectId == projectId)
        .Select(item => new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.MilestoneCreated,
            item.CreatedAtUtc,
            item.Slug,
            item.Title,
            null,
            null,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> milestoneUpdatedItems = await dbContext.Milestones
        .Where(item => item.ProjectId == projectId && item.UpdatedAtUtc > item.CreatedAtUtc)
        .Select(item => new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.MilestoneUpdated,
            item.UpdatedAtUtc,
            item.Slug,
            item.Title,
            null,
            null,
            null,
            null,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> documentationRelationItems = await (
        from relation in dbContext.DocumentationRelations
        join sourceDocumentation in dbContext.Documentations on relation.SourceDocumentationId equals sourceDocumentation.Id
        join targetDocumentation in dbContext.Documentations on relation.TargetDocumentationId equals targetDocumentation.Id
        where relation.ProjectId == projectId
        select new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.DocumentationRelationCreated,
            relation.CreatedAtUtc,
            sourceDocumentation.Slug,
            sourceDocumentation.Title,
            null,
            ProjectActivityFeedDirection.Outgoing,
            targetDocumentation.Slug,
            targetDocumentation.Title,
            relation.Kind,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> projectRelationItems = await (
        from relation in dbContext.ProjectRelations
        join relatedProject in dbContext.Projects on
            (relation.SourceProjectId == projectId ? relation.TargetProjectId : relation.SourceProjectId) equals relatedProject.Id
        where relation.SourceProjectId == projectId || relation.TargetProjectId == projectId
        select new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.ProjectRelationCreated,
            relation.CreatedAtUtc,
            null,
            null,
            null,
            relation.SourceProjectId == projectId
                ? ProjectActivityFeedDirection.Outgoing
                : ProjectActivityFeedDirection.Incoming,
            relatedProject.Slug,
            relatedProject.Title,
            null,
            relation.Kind))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> featureDocumentationLinkItems = await (
        from link in dbContext.FeatureDocumentationLinks
        join feature in dbContext.Features on link.FeatureId equals feature.Id
        join documentation in dbContext.Documentations on link.DocumentationId equals documentation.Id
        where link.ProjectId == projectId
        select new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.FeatureDocumentationLinkCreated,
            link.CreatedAtUtc,
            feature.Slug,
            feature.Title,
            null,
            null,
            documentation.Slug,
            documentation.Title,
            null,
            null))
        .ToListAsync(cancellationToken);

    List<ProjectActivityFeedItem> milestoneFeatureLinkItems = await (
        from link in dbContext.MilestoneFeatureLinks
        join milestone in dbContext.Milestones on link.MilestoneId equals milestone.Id
        join feature in dbContext.Features on link.FeatureId equals feature.Id
        where link.ProjectId == projectId
        select new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.MilestoneFeatureLinkCreated,
            link.CreatedAtUtc,
            milestone.Slug,
            milestone.Title,
            null,
            null,
            feature.Slug,
            feature.Title,
            null,
            null))
        .ToListAsync(cancellationToken);

    return auditItems
        .Concat(featureCreatedItems)
        .Concat(featureUpdatedItems)
        .Concat(milestoneCreatedItems)
        .Concat(milestoneUpdatedItems)
        .Concat(documentationRelationItems)
        .Concat(projectRelationItems)
        .Concat(featureDocumentationLinkItems)
        .Concat(milestoneFeatureLinkItems)
        .OrderByDescending(item => item.OccurredAtUtc)
        .ToList();
  }
}
