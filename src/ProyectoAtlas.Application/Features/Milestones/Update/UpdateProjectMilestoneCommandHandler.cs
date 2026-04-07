using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Milestones.Update;

public class UpdateProjectMilestoneCommandHandler(
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task<Milestone> Execute(
      string projectSlug,
      string slug,
      UpdateProjectMilestoneCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Milestone milestone = await milestoneRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new MilestoneNotFoundException(projectSlug, slug);

    milestone.Update(input.Title, input.Summary, input.Status, input.TargetDateUtc, input.ClearTargetDate);

    await milestoneRepository.Update(milestone, cancellationToken);

    return milestone;
  }
}
