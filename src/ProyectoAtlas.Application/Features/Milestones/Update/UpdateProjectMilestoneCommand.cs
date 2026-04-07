using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.Milestones.Update;

public record UpdateProjectMilestoneCommand(
    string? Title,
    string? Summary,
    MilestoneStatus? Status,
    DateTime? TargetDateUtc = null,
    bool ClearTargetDate = false);
