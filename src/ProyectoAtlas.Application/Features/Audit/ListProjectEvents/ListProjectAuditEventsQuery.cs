using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Audit.ListProjectEvents;

public record ListProjectAuditEventsQuery(
    AuditEntityType? EntityType = null,
    AuditAction? Action = null,
    DateTime? OccurredFromUtc = null,
    DateTime? OccurredToUtc = null,
    int? Limit = null)
    : AuditEventFilters(EntityType, Action, OccurredFromUtc, OccurredToUtc, Limit);
