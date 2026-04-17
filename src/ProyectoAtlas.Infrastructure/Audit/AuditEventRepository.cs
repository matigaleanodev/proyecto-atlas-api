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
      CancellationToken cancellationToken = default)
  {
    return await dbContext.AuditEvents
        .Where(auditEvent => auditEvent.ProjectId == projectId)
        .OrderByDescending(auditEvent => auditEvent.OccurredAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<AuditEvent>> GetDocumentationEvents(
      Guid projectId,
      Guid documentationId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.AuditEvents
        .Where(auditEvent =>
            auditEvent.ProjectId == projectId &&
            auditEvent.DocumentationId == documentationId)
        .OrderByDescending(auditEvent => auditEvent.OccurredAtUtc)
        .ToListAsync(cancellationToken);
  }
}
