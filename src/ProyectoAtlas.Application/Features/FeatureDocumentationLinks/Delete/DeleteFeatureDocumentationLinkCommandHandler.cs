using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.Delete;

public class DeleteFeatureDocumentationLinkCommandHandler(
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      Guid linkId,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    FeatureDocumentationLink link = await featureDocumentationLinkRepository.GetById(project.Id, linkId, cancellationToken)
        ?? throw new FeatureDocumentationLinkNotFoundException(linkId);

    await featureDocumentationLinkRepository.Delete(link, cancellationToken);
  }
}
