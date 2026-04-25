using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Tests.Features.MilestoneFeatureLinks.Common;

internal sealed class FakeMilestoneFeatureLinkRepository : IMilestoneFeatureLinkRepository
{
  public MilestoneFeatureLink? AddedLink { get; private set; }
  public MilestoneFeatureLink? DeletedLink { get; private set; }
  public Guid ReceivedMilestoneId { get; private set; }
  public Guid ReceivedFeatureId { get; private set; }
  public Guid ReceivedLinkId { get; private set; }
  public IReadOnlyCollection<MilestoneFeatureLink> MilestoneLinks { get; set; } = [];
  public IReadOnlyCollection<MilestoneFeatureLink> FeatureLinks { get; set; } = [];
  public MilestoneFeatureLink? LinkById { get; set; }

  public Task Add(MilestoneFeatureLink link, CancellationToken cancellationToken = default)
  {
    AddedLink = link;
    return Task.CompletedTask;
  }

  public Task<IReadOnlyCollection<MilestoneFeatureLink>> GetByMilestoneId(Guid milestoneId, CancellationToken cancellationToken = default)
  {
    ReceivedMilestoneId = milestoneId;
    return Task.FromResult(MilestoneLinks);
  }

  public Task<IReadOnlyCollection<MilestoneFeatureLink>> GetByFeatureId(Guid featureId, CancellationToken cancellationToken = default)
  {
    ReceivedFeatureId = featureId;
    return Task.FromResult(FeatureLinks);
  }

  public Task<MilestoneFeatureLink?> GetById(Guid milestoneId, Guid linkId, CancellationToken cancellationToken = default)
  {
    ReceivedMilestoneId = milestoneId;
    ReceivedLinkId = linkId;
    return Task.FromResult(LinkById);
  }

  public Task Delete(MilestoneFeatureLink link, CancellationToken cancellationToken = default)
  {
    DeletedLink = link;
    return Task.CompletedTask;
  }
}
