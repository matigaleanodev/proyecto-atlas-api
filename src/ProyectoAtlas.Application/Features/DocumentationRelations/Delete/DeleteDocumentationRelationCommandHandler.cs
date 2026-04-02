using ProyectoAtlas.Application.Features.DocumentationRelations.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Delete;

public class DeleteDocumentationRelationCommandHandler(
    IDocumentationRelationRepository documentationRelationRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string projectSlug,
      string sourceDocumentationSlug,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(sourceDocumentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation sourceDocumentation = await documentationRepository.GetBySlug(project.Id, sourceDocumentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, sourceDocumentationSlug);

    DocumentationRelation relation = await documentationRelationRepository.GetById(
        sourceDocumentation.Id,
        relationId,
        cancellationToken)
        ?? throw new DocumentationRelationNotFoundException(projectSlug, sourceDocumentationSlug, relationId);

    await documentationRelationRepository.Delete(relation, cancellationToken);
  }
}
