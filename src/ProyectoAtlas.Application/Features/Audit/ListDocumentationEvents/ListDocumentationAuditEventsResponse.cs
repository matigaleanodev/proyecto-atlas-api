using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Audit.ListDocumentationEvents;

public record ListDocumentationAuditEventsResponse(IReadOnlyCollection<AuditEvent> Items);
