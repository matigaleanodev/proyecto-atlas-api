using ProyectoAtlas.Application.Features.DocumentationRelations.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.ListIncoming;

public class ListIncomingDocumentationRelationsQueryHandler(
    IDocumentationRelationRepository documentationRelationRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListDocumentationRelationsResponse> Execute(
      string projectSlug,
      string targetDocumentationSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(targetDocumentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation targetDocumentation = await documentationRepository.GetBySlug(project.Id, targetDocumentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, targetDocumentationSlug);

    IReadOnlyCollection<DocumentationRelation> relations = await documentationRelationRepository.GetIncomingList(
        targetDocumentation.Id,
        cancellationToken);

    return new ListDocumentationRelationsResponse(relations);
  }
}
