using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.ListByMilestone;

public record ListMilestoneFeatureLinksResponse(IReadOnlyCollection<MilestoneFeatureLink> Items);
