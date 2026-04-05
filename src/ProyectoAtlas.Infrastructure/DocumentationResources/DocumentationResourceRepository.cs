using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.DocumentationResources;

public class DocumentationResourceRepository(ProyectoAtlasDbContext dbContext) : IDocumentationResourceRepository
{
  public async Task Add(DocumentationResource resource, CancellationToken cancellationToken = default)
  {
    await dbContext.DocumentationResources.AddAsync(resource, cancellationToken);

    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (
        exception.InnerException is PostgresException postgresException &&
        postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
        postgresException.ConstraintName == "IX_documentation_resources_documentation_id_normalized_title_normalized_url_kind")
    {
      throw new DuplicateDocumentationResourceException(resource.Title);
    }
  }

  public async Task<IReadOnlyCollection<DocumentationResource>> GetList(
      Guid documentationId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.DocumentationResources
        .Where(resource => resource.DocumentationId == documentationId)
        .OrderBy(resource => resource.Title)
        .ToListAsync(cancellationToken);
  }

  public async Task<DocumentationResource?> GetById(
      Guid documentationId,
      Guid resourceId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.DocumentationResources
        .FirstOrDefaultAsync(
            resource => resource.DocumentationId == documentationId && resource.Id == resourceId,
            cancellationToken);
  }

  public async Task Delete(DocumentationResource resource, CancellationToken cancellationToken = default)
  {
    dbContext.DocumentationResources.Remove(resource);
    await dbContext.SaveChangesAsync(cancellationToken);
  }
}
