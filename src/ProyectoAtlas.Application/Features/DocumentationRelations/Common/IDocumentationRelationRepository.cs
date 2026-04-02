using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Common;

public interface IDocumentationRelationRepository
{
  Task Add(DocumentationRelation relation, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<DocumentationRelation>> GetOutgoingList(
      Guid sourceDocumentationId,
      CancellationToken cancellationToken = default);

  Task<DocumentationRelation?> GetById(
      Guid sourceDocumentationId,
      Guid relationId,
      CancellationToken cancellationToken = default);

  Task Delete(DocumentationRelation relation, CancellationToken cancellationToken = default);
}
