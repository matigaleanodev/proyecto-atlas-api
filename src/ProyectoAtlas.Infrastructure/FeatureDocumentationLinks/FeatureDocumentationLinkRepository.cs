using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.FeatureDocumentationLinks;

public class FeatureDocumentationLinkRepository(ProyectoAtlasDbContext dbContext) : IFeatureDocumentationLinkRepository
{
  public async Task Add(FeatureDocumentationLink link, CancellationToken cancellationToken = default)
  {
    await dbContext.FeatureDocumentationLinks.AddAsync(link, cancellationToken);
    await SaveChanges(cancellationToken);
  }

  public async Task<IReadOnlyCollection<FeatureDocumentationLink>> GetByFeatureId(Guid featureId, CancellationToken cancellationToken = default)
  {
    return await dbContext.FeatureDocumentationLinks
        .Where(link => link.FeatureId == featureId)
        .OrderBy(link => link.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<FeatureDocumentationLink>> GetByDocumentationId(Guid documentationId, CancellationToken cancellationToken = default)
  {
    return await dbContext.FeatureDocumentationLinks
        .Where(link => link.DocumentationId == documentationId)
        .OrderBy(link => link.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<FeatureDocumentationLink?> GetById(Guid featureId, Guid linkId, CancellationToken cancellationToken = default)
  {
    return await dbContext.FeatureDocumentationLinks
        .FirstOrDefaultAsync(link => link.FeatureId == featureId && link.Id == linkId, cancellationToken);
  }

  public async Task Delete(FeatureDocumentationLink link, CancellationToken cancellationToken = default)
  {
    dbContext.FeatureDocumentationLinks.Remove(link);
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
        postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
    {
      throw new DuplicateFeatureDocumentationLinkException();
    }
  }
}
