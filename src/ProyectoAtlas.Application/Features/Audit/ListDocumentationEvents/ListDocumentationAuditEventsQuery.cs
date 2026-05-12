using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Audit.ListDocumentationEvents;

public record ListDocumentationAuditEventsQuery(
    AuditEntityType? EntityType = null,
    AuditAction? Action = null,
    DateTime? OccurredFromUtc = null,
    DateTime? OccurredToUtc = null,
    int? Limit = null)
    : AuditEventFilters(EntityType, Action, OccurredFromUtc, OccurredToUtc, Limit);
