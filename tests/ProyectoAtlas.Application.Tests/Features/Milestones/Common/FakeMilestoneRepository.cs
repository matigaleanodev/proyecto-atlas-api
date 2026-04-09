using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Tests.Features.Milestones.Common;

internal sealed class FakeMilestoneRepository : IMilestoneRepository
{
  public Milestone? AddedMilestone { get; private set; }
  public Milestone? UpdatedMilestone { get; private set; }
  public Milestone? DeletedMilestone { get; private set; }
  public Guid ReceivedProjectId { get; private set; }
  public int ReceivedPage { get; private set; }
  public int ReceivedPageSize { get; private set; }
  public string? ReceivedQuery { get; private set; }
  public MilestoneStatus? ReceivedStatus { get; private set; }
  public IEnumerable<Milestone> PagedMilestones { get; set; } = [];
  public int PagedTotalCount { get; set; }
  public Milestone? MilestoneBySlug { get; set; }

  public Task Add(Milestone milestone, CancellationToken cancellationToken = default)
  {
    AddedMilestone = milestone;
    return Task.CompletedTask;
  }

  public Task<(IEnumerable<Milestone> Milestones, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      MilestoneStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    ReceivedPage = page;
    ReceivedPageSize = pageSize;
    ReceivedQuery = query;
    ReceivedStatus = status;

    return Task.FromResult((PagedMilestones, PagedTotalCount));
  }

  public Task<Milestone?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    return Task.FromResult(MilestoneBySlug);
  }

  public Task Update(Milestone milestone, CancellationToken cancellationToken = default)
  {
    UpdatedMilestone = milestone;
    return Task.CompletedTask;
  }

  public Task Delete(Milestone milestone, CancellationToken cancellationToken = default)
  {
    DeletedMilestone = milestone;
    return Task.CompletedTask;
  }
}
