using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Documentations;

public class CreateProjectDocumentationUseCase(
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(
      string projectSlug,
      CreateProjectDocumentationInput input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.ContentMarkdown);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = new(
        project.Id,
        input.Title,
        input.ContentMarkdown,
        input.SortOrder,
        input.Kind);

    await documentationRepository.Add(documentation, cancellationToken);

    return documentation;
  }
}
