using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Create;

public class CreateDocumentationResourceCommandHandler(
    IDocumentationResourceRepository documentationResourceRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<DocumentationResource> Execute(
      string projectSlug,
      string documentationSlug,
      CreateDocumentationResourceCommand command,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(documentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, documentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, documentationSlug);

    DocumentationResource resource;

    try
    {
      resource = new DocumentationResource(
          documentation.Id,
          command.Title,
          command.Url,
          command.Kind);
    }
    catch (InvalidDocumentationResourceException exception)
    {
      throw new InvalidDocumentationResourceItemException(exception.Message);
    }

    await documentationResourceRepository.Add(resource, cancellationToken);

    return resource;
  }
}
