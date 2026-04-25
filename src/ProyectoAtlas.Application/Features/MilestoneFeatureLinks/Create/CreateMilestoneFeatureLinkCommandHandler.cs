using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.Create;

public class CreateMilestoneFeatureLinkCommandHandler(
    IMilestoneFeatureLinkRepository milestoneFeatureLinkRepository,
    IFeatureRepository featureRepository,
    IMilestoneRepository milestoneRepository,
    IProjectRepository projectRepository)
{
  public async Task<MilestoneFeatureLink> Execute(
      string projectSlug,
      string milestoneSlug,
      CreateMilestoneFeatureLinkCommand command,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(milestoneSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(command.FeatureSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Milestone milestone = await milestoneRepository.GetBySlug(project.Id, milestoneSlug, cancellationToken)
        ?? throw new MilestoneNotFoundException(projectSlug, milestoneSlug);

    Feature feature = await featureRepository.GetBySlug(project.Id, command.FeatureSlug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, command.FeatureSlug);

    MilestoneFeatureLink link = new(project.Id, milestone.Id, feature.Id);
    await milestoneFeatureLinkRepository.Add(link, cancellationToken);

    return link;
  }
}
