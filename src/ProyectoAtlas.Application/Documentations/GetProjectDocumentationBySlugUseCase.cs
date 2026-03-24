using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Documentations;

public class GetProjectDocumentationBySlugUseCase(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(string projectSlug, string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
    ?? throw new KeyNotFoundException($"Project with slug '{projectSlug}' not found.");

    return await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
    ?? throw new KeyNotFoundException($"Documentation with slug '{slug}' not found for project '{projectSlug}'.");
  }
}