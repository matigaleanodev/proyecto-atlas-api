using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Milestones.Delete;

public class DeleteProjectMilestoneCommandHandler(
    IMilestoneRepository milestoneRepository,
    IMilestoneFeatureLinkRepository milestoneFeatureLinkRepository,
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

    IReadOnlyCollection<Domain.Milestones.MilestoneFeatureLink> featureLinks =
        await milestoneFeatureLinkRepository.GetByMilestoneId(milestone.Id, cancellationToken);

    if (featureLinks.Count > 0)
    {
      throw new MilestoneDeleteBlockedException(projectSlug, slug, "it is still linked to one or more features");
    }

    await milestoneRepository.Delete(milestone, cancellationToken);
  }
}
