using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Delete;

public class DeleteDocumentationResourceCommandHandler(
    IDocumentationResourceRepository documentationResourceRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      string documentationSlug,
      Guid resourceId,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(documentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, documentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, documentationSlug);

    DocumentationResource resource = await documentationResourceRepository.GetById(
        documentation.Id,
        resourceId,
        cancellationToken)
        ?? throw new DocumentationResourceNotFoundException(resourceId);

    await documentationResourceRepository.Delete(resource, cancellationToken);
  }
}
