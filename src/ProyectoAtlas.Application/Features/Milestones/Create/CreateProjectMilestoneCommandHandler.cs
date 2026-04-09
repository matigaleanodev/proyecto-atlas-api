using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Milestones.Create;

public class CreateProjectMilestoneCommandHandler(
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task<Milestone> Execute(
      string projectSlug,
      CreateProjectMilestoneCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Summary);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Milestone milestone = new(project.Id, input.Title, input.Summary, input.Status, input.TargetDateUtc);

    await milestoneRepository.Add(milestone, cancellationToken);

    return milestone;
  }
}
