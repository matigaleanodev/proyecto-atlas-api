using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.Create;

public class CreateProjectDocumentationCommandHandler(
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(
      string projectSlug,
      CreateProjectDocumentationCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.ContentMarkdown);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation;

    try
    {
      documentation = new(
          project.Id,
          input.Title,
          input.ContentMarkdown,
          input.SortOrder,
          input.Kind,
          input.Status);
    }
    catch (InvalidDocumentationTitleException exception)
    {
      throw new InvalidDocumentationTitleConventionException(exception.Message);
    }

    await documentationRepository.Add(documentation, cancellationToken);

    return documentation;
  }
}
