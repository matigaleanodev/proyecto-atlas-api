using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.Common;

public interface IMilestoneFeatureLinkRepository
{
  Task Add(MilestoneFeatureLink link, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<MilestoneFeatureLink>> GetByMilestoneId(Guid milestoneId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<MilestoneFeatureLink>> GetByFeatureId(Guid featureId, CancellationToken cancellationToken = default);
  Task<MilestoneFeatureLink?> GetById(Guid milestoneId, Guid linkId, CancellationToken cancellationToken = default);
  Task Delete(MilestoneFeatureLink link, CancellationToken cancellationToken = default);
}
