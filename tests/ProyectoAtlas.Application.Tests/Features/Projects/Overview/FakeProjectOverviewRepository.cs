using ProyectoAtlas.Application.Features.Projects.Overview;
using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Application.Tests.Features.Projects.Overview;

internal sealed class FakeProjectOverviewRepository : IProjectOverviewRepository
{
  public Guid ReceivedProjectId { get; private set; }

  public ProjectOverviewSummary Overview { get; set; } = new(
      Guid.Empty,
      string.Empty,
      string.Empty,
      0,
      0,
      0,
      0,
      0,
      Array.Empty<AuditEvent>());

  public Task<ProjectOverviewSummary> GetByProjectId(Guid projectId, CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    return Task.FromResult(Overview);
  }
}
