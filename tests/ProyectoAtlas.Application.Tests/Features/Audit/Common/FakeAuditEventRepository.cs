using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Tests.Features.Audit.Common;

internal sealed class FakeAuditEventRepository : IAuditEventRepository
{
  public AuditEvent? AddedAuditEvent { get; private set; }
  public Guid ReceivedProjectId { get; private set; }
  public Guid ReceivedDocumentationId { get; private set; }
  public AuditEventFilters? ReceivedFilters { get; private set; }
  public IReadOnlyCollection<AuditEvent> ProjectEvents { get; set; } = [];
  public IReadOnlyCollection<AuditEvent> DocumentationEvents { get; set; } = [];

  public Task Add(AuditEvent auditEvent, CancellationToken cancellationToken = default)
  {
    AddedAuditEvent = auditEvent;
    return Task.CompletedTask;
  }

  public Task<IReadOnlyCollection<AuditEvent>> GetProjectEvents(
      Guid projectId,
      AuditEventFilters filters,
      CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    ReceivedFilters = filters;
    return Task.FromResult(ProjectEvents);
  }

  public Task<IReadOnlyCollection<AuditEvent>> GetDocumentationEvents(
      Guid projectId,
      Guid documentationId,
      AuditEventFilters filters,
      CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    ReceivedDocumentationId = documentationId;
    ReceivedFilters = filters;
    return Task.FromResult(DocumentationEvents);
  }
}
