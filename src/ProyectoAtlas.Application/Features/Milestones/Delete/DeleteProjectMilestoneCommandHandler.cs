using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Milestones.Delete;

public class DeleteProjectMilestoneCommandHandler(
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Domain.Milestones.Milestone milestone = await milestoneRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new MilestoneNotFoundException(projectSlug, slug);

    await milestoneRepository.Delete(milestone, cancellationToken);
  }
}
