using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Common;

public interface IProjectRelationRepository
{
  Task Add(ProjectRelation relation, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ProjectRelation>> GetOutgoingList(Guid sourceProjectId, CancellationToken cancellationToken = default);
  Task<ProjectRelation?> GetById(Guid sourceProjectId, Guid relationId, CancellationToken cancellationToken = default);
  Task Delete(ProjectRelation relation, CancellationToken cancellationToken = default);
}
