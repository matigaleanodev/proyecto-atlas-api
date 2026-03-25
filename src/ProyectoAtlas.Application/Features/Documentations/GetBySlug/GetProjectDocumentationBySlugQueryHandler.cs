using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Documentations;

public class GetProjectDocumentationBySlugQueryHandler(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(string projectSlug, string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    return await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, slug);
  }
}
