using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.ListByMilestone;

public class ListMilestoneFeatureLinksQueryHandler(
    IMilestoneFeatureLinkRepository milestoneFeatureLinkRepository,
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListMilestoneFeatureLinksResponse> Execute(
      string projectSlug,
      string milestoneSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(milestoneSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Milestone milestone = await milestoneRepository.GetBySlug(project.Id, milestoneSlug, cancellationToken)
        ?? throw new MilestoneNotFoundException(projectSlug, milestoneSlug);

    IReadOnlyCollection<MilestoneFeatureLink> links = await milestoneFeatureLinkRepository.GetByMilestoneId(
        milestone.Id,
        cancellationToken);

    return new ListMilestoneFeatureLinksResponse(links);
  }
}
