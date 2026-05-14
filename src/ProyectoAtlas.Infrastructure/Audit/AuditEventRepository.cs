using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Audit;

public class AuditEventRepository(ProyectoAtlasDbContext dbContext) : IAuditEventRepository
{
  public async Task Add(AuditEvent auditEvent, CancellationToken cancellationToken = default)
  {
    await dbContext.AddAsync(auditEvent, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<AuditEvent>> GetProjectEvents(
      Guid projectId,
      AuditEventFilters filters,
      CancellationToken cancellationToken = default)
  {
    IQueryable<AuditEvent> query = dbContext.AuditEvents
        .Where(auditEvent => auditEvent.ProjectId == projectId);

    query = ApplyFilters(query, filters);

    query = query.OrderByDescending(auditEvent => auditEvent.OccurredAtUtc);

    if (filters.Limit.HasValue)
    {
      query = query.Take(filters.Limit.Value);
    }

    return await query
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<AuditEvent>> GetDocumentationEvents(
      Guid projectId,
      Guid documentationId,
      AuditEventFilters filters,
      CancellationToken cancellationToken = default)
  {
    IQueryable<AuditEvent> query = dbContext.AuditEvents
        .Where(auditEvent =>
            auditEvent.ProjectId == projectId &&
            auditEvent.DocumentationId == documentationId);

    query = ApplyFilters(query, filters);

    query = query.OrderByDescending(auditEvent => auditEvent.OccurredAtUtc);

    if (filters.Limit.HasValue)
    {
      query = query.Take(filters.Limit.Value);
    }

    return await query
        .ToListAsync(cancellationToken);
  }

  private static IQueryable<AuditEvent> ApplyFilters(IQueryable<AuditEvent> query, AuditEventFilters filters)
  {
    if (filters.EntityType.HasValue)
    {
      query = query.Where(auditEvent => auditEvent.EntityType == filters.EntityType.Value);
    }

    if (filters.Action.HasValue)
    {
      query = query.Where(auditEvent => auditEvent.Action == filters.Action.Value);
    }

    if (filters.OccurredFromUtc.HasValue)
    {
      query = query.Where(auditEvent => auditEvent.OccurredAtUtc >= filters.OccurredFromUtc.Value);
    }

    if (filters.OccurredToUtc.HasValue)
    {
      query = query.Where(auditEvent => auditEvent.OccurredAtUtc <= filters.OccurredToUtc.Value);
    }

    return query;
  }
}
