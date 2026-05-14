namespace ProyectoAtlas.Application.Features.Projects.Overview;

public interface IProjectOverviewRepository
{
  Task<ProjectOverviewSummary> GetByProjectId(Guid projectId, CancellationToken cancellationToken = default);
}
