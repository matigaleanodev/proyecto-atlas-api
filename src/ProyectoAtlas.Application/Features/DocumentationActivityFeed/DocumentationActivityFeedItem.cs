using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationActivityFeed;

public record DocumentationActivityFeedItem(
    DocumentationActivityFeedItemType Type,
    DateTime OccurredAtUtc,
    AuditAction? AuditAction = null,
    string? EntitySlug = null,
    string? EntityTitle = null,
    int? VersionNumber = null,
    DocumentationStatus? VersionStatus = null,
    DocumentationRelationKind? RelationKind = null,
    DocumentationActivityRelationDirection? RelationDirection = null,
    string? RelatedDocumentationSlug = null,
    string? RelatedDocumentationTitle = null);
