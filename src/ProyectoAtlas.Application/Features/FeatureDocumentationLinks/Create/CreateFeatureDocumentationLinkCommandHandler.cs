using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.Create;

public class CreateFeatureDocumentationLinkCommandHandler(
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IFeatureRepository featureRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<FeatureDocumentationLink> Execute(
      string projectSlug,
      string featureSlug,
      CreateFeatureDocumentationLinkCommand command,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(featureSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(command.DocumentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Feature feature = await featureRepository.GetBySlug(project.Id, featureSlug, cancellationToken)
        ?? throw new FeatureNotFoundException(projectSlug, featureSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, command.DocumentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, command.DocumentationSlug);

    FeatureDocumentationLink link = new(project.Id, feature.Id, documentation.Id);
    await featureDocumentationLinkRepository.Add(link, cancellationToken);

    return link;
  }
}
