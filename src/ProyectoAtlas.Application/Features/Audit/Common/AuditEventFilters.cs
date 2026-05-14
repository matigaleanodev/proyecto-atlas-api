using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Audit.Common;

public record AuditEventFilters(
    AuditEntityType? EntityType = null,
    AuditAction? Action = null,
    DateTime? OccurredFromUtc = null,
    DateTime? OccurredToUtc = null,
    int? Limit = null);
