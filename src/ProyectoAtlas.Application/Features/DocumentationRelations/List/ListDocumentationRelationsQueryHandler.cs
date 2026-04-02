using ProyectoAtlas.Application.Features.DocumentationRelations.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.List;

public class ListDocumentationRelationsQueryHandler(
    IDocumentationRelationRepository documentationRelationRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListDocumentationRelationsResponse> Execute(
      string projectSlug,
      string sourceDocumentationSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(sourceDocumentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation sourceDocumentation = await documentationRepository.GetBySlug(project.Id, sourceDocumentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, sourceDocumentationSlug);

    IReadOnlyCollection<DocumentationRelation> relations = await documentationRelationRepository.GetOutgoingList(
        sourceDocumentation.Id,
        cancellationToken);

    return new ListDocumentationRelationsResponse(relations);
  }
}
