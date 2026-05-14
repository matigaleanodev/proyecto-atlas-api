using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Overview;

public class GetProjectOverviewQueryHandler(
    IProjectOverviewRepository projectOverviewRepository,
    IProjectRepository projectRepository)
{
  public async Task<ProjectOverviewSummary> Execute(
      string projectSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    return await projectOverviewRepository.GetByProjectId(project.Id, cancellationToken);
  }
}
