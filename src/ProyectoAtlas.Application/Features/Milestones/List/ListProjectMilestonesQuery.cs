using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.Milestones.List;

public record ListProjectMilestonesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Query = null,
    MilestoneStatus? Status = null);
