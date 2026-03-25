using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.Delete;

public class DeleteProjectDocumentationCommandHandler(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task Execute(string projectSlug, string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, slug);

    await documentationRepository.Delete(documentation, cancellationToken);
  }
}
