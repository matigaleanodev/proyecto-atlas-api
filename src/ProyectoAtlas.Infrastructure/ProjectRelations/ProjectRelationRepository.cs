using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.ProjectRelations;

public class ProjectRelationRepository(ProyectoAtlasDbContext dbContext) : IProjectRelationRepository
{
  public async Task Add(ProjectRelation relation, CancellationToken cancellationToken = default)
  {
    await dbContext.ProjectRelations.AddAsync(relation, cancellationToken);
    await SaveChanges(cancellationToken);
  }

  public async Task<IReadOnlyCollection<ProjectRelation>> GetOutgoingList(
      Guid sourceProjectId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.ProjectRelations
        .Where(relation => relation.SourceProjectId == sourceProjectId)
        .OrderBy(relation => relation.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<ProjectRelation?> GetById(
      Guid sourceProjectId,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.ProjectRelations
        .FirstOrDefaultAsync(
            relation => relation.SourceProjectId == sourceProjectId && relation.Id == relationId,
            cancellationToken);
  }

  public async Task Delete(ProjectRelation relation, CancellationToken cancellationToken = default)
  {
    dbContext.ProjectRelations.Remove(relation);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  private async Task SaveChanges(CancellationToken cancellationToken)
  {
    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (
        exception.InnerException is PostgresException postgresException &&
        postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
        postgresException.ConstraintName == "IX_project_relations_source_project_id_target_project_id_kind")
    {
      throw new DuplicateProjectRelationException();
    }
  }
}
