using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectActivityFeed;

public record ProjectActivityFeedItem(
    ProjectActivityFeedItemType Type,
    DateTime OccurredAtUtc,
    string? EntitySlug = null,
    string? EntityTitle = null,
    AuditAction? AuditAction = null,
    ProjectActivityFeedDirection? Direction = null,
    string? RelatedEntitySlug = null,
    string? RelatedEntityTitle = null,
    DocumentationRelationKind? DocumentationRelationKind = null,
    ProjectRelationKind? ProjectRelationKind = null);
