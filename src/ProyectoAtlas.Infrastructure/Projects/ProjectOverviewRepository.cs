using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Application.Features.Projects.Overview;
using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Projects;

public class ProjectOverviewRepository(ProyectoAtlasDbContext dbContext) : IProjectOverviewRepository
{
  private const int RecentActivityLimit = 5;

  public async Task<ProjectOverviewSummary> GetByProjectId(Guid projectId, CancellationToken cancellationToken = default)
  {
    var project = await dbContext.Projects
        .Where(item => item.Id == projectId)
        .Select(item => new
        {
          item.Id,
          item.Slug,
          item.Title
        })
        .FirstAsync(cancellationToken);

    int documentationCount = await dbContext.Documentations
        .CountAsync(item => item.ProjectId == projectId, cancellationToken);

    int featureCount = await dbContext.Features
        .CountAsync(item => item.ProjectId == projectId, cancellationToken);

    int milestoneCount = await dbContext.Milestones
        .CountAsync(item => item.ProjectId == projectId, cancellationToken);

    int outgoingRelationCount = await dbContext.ProjectRelations
        .CountAsync(item => item.SourceProjectId == projectId, cancellationToken);

    int incomingRelationCount = await dbContext.ProjectRelations
        .CountAsync(item => item.TargetProjectId == projectId, cancellationToken);

    List<AuditEvent> recentActivity = await dbContext.AuditEvents
        .Where(item => item.ProjectId == projectId)
        .OrderByDescending(item => item.OccurredAtUtc)
        .Take(RecentActivityLimit)
        .ToListAsync(cancellationToken);

    return new ProjectOverviewSummary(
        project.Id,
        project.Slug,
        project.Title,
        documentationCount,
        featureCount,
        milestoneCount,
        outgoingRelationCount,
        incomingRelationCount,
        recentActivity);
  }
}
