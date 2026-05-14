using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.Delete;

public class DeleteMilestoneFeatureLinkCommandHandler(
    IMilestoneFeatureLinkRepository milestoneFeatureLinkRepository,
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      string milestoneSlug,
      Guid linkId,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(milestoneSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Milestone milestone = await milestoneRepository.GetBySlug(project.Id, milestoneSlug, cancellationToken)
        ?? throw new MilestoneNotFoundException(projectSlug, milestoneSlug);

    MilestoneFeatureLink link = await milestoneFeatureLinkRepository.GetById(milestone.Id, linkId, cancellationToken)
        ?? throw new MilestoneFeatureLinkNotFoundException(linkId);

    await milestoneFeatureLinkRepository.Delete(link, cancellationToken);
  }
}
