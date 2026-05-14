using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.ListByFeature;

public record ListFeatureMilestoneLinksResponse(IReadOnlyCollection<MilestoneFeatureLink> Items);
