using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationRelations.Common;

internal sealed class FakeDocumentationRelationRepository : IDocumentationRelationRepository
{
  public DocumentationRelation? AddedRelation { get; private set; }
  public DocumentationRelation? DeletedRelation { get; private set; }
  public Guid ReceivedSourceDocumentationId { get; private set; }
  public Guid ReceivedRelationId { get; private set; }
  public IReadOnlyCollection<DocumentationRelation> OutgoingRelations { get; set; } = [];
  public DocumentationRelation? RelationById { get; set; }

  public Task Add(DocumentationRelation relation, CancellationToken cancellationToken = default)
  {
    AddedRelation = relation;
    return Task.CompletedTask;
  }

  public Task<IReadOnlyCollection<DocumentationRelation>> GetOutgoingList(
      Guid sourceDocumentationId,
      CancellationToken cancellationToken = default)
  {
    ReceivedSourceDocumentationId = sourceDocumentationId;
    return Task.FromResult(OutgoingRelations);
  }

  public Task<DocumentationRelation?> GetById(
      Guid sourceDocumentationId,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    ReceivedSourceDocumentationId = sourceDocumentationId;
    ReceivedRelationId = relationId;
    return Task.FromResult(RelationById);
  }

  public Task Delete(DocumentationRelation relation, CancellationToken cancellationToken = default)
  {
    DeletedRelation = relation;
    return Task.CompletedTask;
  }
}
