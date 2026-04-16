using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Audit.Common;

public interface IAuditEventRepository
{
  Task Add(AuditEvent auditEvent, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<AuditEvent>> GetProjectEvents(Guid projectId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<AuditEvent>> GetDocumentationEvents(Guid projectId, Guid documentationId, CancellationToken cancellationToken = default);
}
