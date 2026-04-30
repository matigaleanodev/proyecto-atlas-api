using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Features.Projects.Overview;

public record ProjectOverviewSummary(
    Guid ProjectId,
    string ProjectSlug,
    string ProjectTitle,
    int DocumentationCount,
    int FeatureCount,
    int MilestoneCount,
    int OutgoingRelationCount,
    int IncomingRelationCount,
    IReadOnlyCollection<AuditEvent> RecentActivity);
