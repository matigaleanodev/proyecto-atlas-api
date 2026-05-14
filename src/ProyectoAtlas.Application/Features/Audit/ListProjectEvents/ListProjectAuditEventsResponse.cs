using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Audit.ListProjectEvents;

public record ListProjectAuditEventsResponse(IReadOnlyCollection<AuditEvent> Items);
