using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Documentations;

public class DocumentationRepository(ProyectoAtlasDbContext dbContext) : IDocumentationRepository
{
  public async Task Add(Documentation documentation, CancellationToken cancellationToken = default)
  {
    await dbContext.Documentations.AddAsync(documentation, cancellationToken);
    await SaveChanges(documentation.Slug, cancellationToken);
  }
  public async Task<(IEnumerable<Documentation> Documentations, int TotalCount)> GetPagedList(Guid projectId, int page, int pageSize, string? query = null, CancellationToken cancellationToken = default)
  {
    IQueryable<Documentation> documentationsQuery = dbContext.Documentations
        .Where(documentation => documentation.ProjectId == projectId);

    if (!string.IsNullOrWhiteSpace(query))
    {
      string normalizedQuery = $"%{query.Trim()}%";

      documentationsQuery = documentationsQuery.Where(documentation =>
          EF.Functions.ILike(documentation.Title, normalizedQuery));
    }
    int totalCount = await documentationsQuery.CountAsync(cancellationToken);

    List<Documentation> documentations = await documentationsQuery
        .OrderBy(documentation => documentation.SortOrder)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return (documentations, totalCount);
  }
  public async Task<Documentation?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    return await dbContext.Documentations
        .FirstOrDefaultAsync(documentation => documentation.ProjectId == projectId && documentation.Slug == slug, cancellationToken);
  }

  public async Task Update(Documentation documentation, CancellationToken cancellationToken = default)
  {
    dbContext.Documentations.Update(documentation);
    await SaveChanges(documentation.Slug, cancellationToken);
  }

  public async Task Delete(Documentation documentation, CancellationToken cancellationToken = default)
  {
    dbContext.Documentations.Remove(documentation);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  private async Task SaveChanges(string slug, CancellationToken cancellationToken)
  {
    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (
        exception.InnerException is PostgresException postgresException &&
        postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
        postgresException.ConstraintName == "IX_documentations_project_id_slug")
    {
      throw new DuplicateDocumentationSlugException(slug);
    }
  }
}


