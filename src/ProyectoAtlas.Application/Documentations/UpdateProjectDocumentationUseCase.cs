using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Documentations;

public class UpdateProjectDocumentationUseCase(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(string projectSlug, string slug, UpdateProjectDocumentationInput input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, slug);

    documentation.Update(
      title: input.Title,
      contentMarkdown: input.ContentMarkdown,
      sortOrder: input.SortOrder,
      kind: input.Kind,
      status: input.Status
    );

    await documentationRepository.Update(documentation, cancellationToken);

    return documentation;
  }
}
