using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationActivityFeed;

public class GetDocumentationActivityFeedQueryHandler(
    IDocumentationActivityFeedRepository documentationActivityFeedRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<GetDocumentationActivityFeedResponse> Execute(
      string projectSlug,
      string documentationSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(documentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, documentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, documentationSlug);

    IReadOnlyCollection<DocumentationActivityFeedItem> items = await documentationActivityFeedRepository.GetItems(
        documentation.Id,
        cancellationToken);

    return new GetDocumentationActivityFeedResponse(items);
  }
}
