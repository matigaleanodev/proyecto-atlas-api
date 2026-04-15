using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ProjectRelations.Common;

internal sealed class FakeProjectRelationRepository : IProjectRelationRepository
{
  public ProjectRelation? AddedRelation { get; private set; }
  public ProjectRelation? DeletedRelation { get; private set; }
  public Guid ReceivedSourceProjectId { get; private set; }
  public Guid ReceivedRelationId { get; private set; }
  public IReadOnlyCollection<ProjectRelation> OutgoingRelations { get; set; } = [];
  public ProjectRelation? RelationById { get; set; }

  public Task Add(ProjectRelation relation, CancellationToken cancellationToken = default)
  {
    AddedRelation = relation;
    return Task.CompletedTask;
  }

  public Task<IReadOnlyCollection<ProjectRelation>> GetOutgoingList(
      Guid sourceProjectId,
      CancellationToken cancellationToken = default)
  {
    ReceivedSourceProjectId = sourceProjectId;
    return Task.FromResult(OutgoingRelations);
  }

  public Task<ProjectRelation?> GetById(
      Guid sourceProjectId,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    ReceivedSourceProjectId = sourceProjectId;
    ReceivedRelationId = relationId;
    return Task.FromResult(RelationById);
  }

  public Task Delete(ProjectRelation relation, CancellationToken cancellationToken = default)
  {
    DeletedRelation = relation;
    return Task.CompletedTask;
  }
}
