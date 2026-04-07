using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Application.Features.Milestones.Common;

public interface IMilestoneRepository
{
  Task Add(Milestone milestone, CancellationToken cancellationToken = default);
  Task<(IEnumerable<Milestone> Milestones, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      MilestoneStatus? status = null,
      CancellationToken cancellationToken = default);
  Task<Milestone?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default);
  Task Update(Milestone milestone, CancellationToken cancellationToken = default);
  Task Delete(Milestone milestone, CancellationToken cancellationToken = default);
}
