using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.Milestones.List;

public record ListProjectMilestonesResponse(
    IReadOnlyCollection<Milestone> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
