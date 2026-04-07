using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Milestones.GetBySlug;

public class GetProjectMilestoneBySlugQueryHandler(
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task<Milestone> Execute(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    return await milestoneRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new MilestoneNotFoundException(projectSlug, slug);
  }
}
