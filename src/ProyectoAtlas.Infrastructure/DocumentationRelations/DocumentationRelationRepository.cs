using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.DocumentationRelations;

public class DocumentationRelationRepository(ProyectoAtlasDbContext dbContext) : IDocumentationRelationRepository
{
  public async Task Add(DocumentationRelation relation, CancellationToken cancellationToken = default)
  {
    await dbContext.DocumentationRelations.AddAsync(relation, cancellationToken);
    await SaveChanges(cancellationToken);
  }

  public async Task<IReadOnlyCollection<DocumentationRelation>> GetOutgoingList(
      Guid sourceDocumentationId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.DocumentationRelations
        .Where(relation => relation.SourceDocumentationId == sourceDocumentationId)
        .OrderBy(relation => relation.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<DocumentationRelation?> GetById(
      Guid sourceDocumentationId,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.DocumentationRelations
        .FirstOrDefaultAsync(
            relation => relation.SourceDocumentationId == sourceDocumentationId && relation.Id == relationId,
            cancellationToken);
  }

  public async Task Delete(DocumentationRelation relation, CancellationToken cancellationToken = default)
  {
    dbContext.DocumentationRelations.Remove(relation);
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
        postgresException.ConstraintName == "IX_documentation_relations_project_id_source_documentation_id_~")
    {
      throw new DuplicateDocumentationRelationException();
    }
  }
}
