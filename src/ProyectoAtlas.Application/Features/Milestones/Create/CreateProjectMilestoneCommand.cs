using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.Milestones.Create;

public record CreateProjectMilestoneCommand(
    string Title,
    string Summary,
    MilestoneStatus Status,
    DateTime? TargetDateUtc = null);
