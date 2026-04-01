using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Features;

public class FeatureRepository(ProyectoAtlasDbContext dbContext) : IFeatureRepository
{
  public async Task Add(Feature feature, CancellationToken cancellationToken = default)
  {
    await dbContext.Features.AddAsync(feature, cancellationToken);
    await SaveChanges(feature.Slug, cancellationToken);
  }

  public async Task<(IEnumerable<Feature> Features, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      FeatureStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    IQueryable<Feature> featuresQuery = dbContext.Features
        .Where(feature => feature.ProjectId == projectId);

    if (!string.IsNullOrWhiteSpace(query))
    {
      string normalizedQuery = $"%{query.Trim()}%";

      featuresQuery = featuresQuery.Where(feature =>
          EF.Functions.ILike(feature.Title, normalizedQuery) ||
          EF.Functions.ILike(feature.Summary, normalizedQuery));
    }

    if (status.HasValue)
    {
      featuresQuery = featuresQuery.Where(feature => feature.Status == status.Value);
    }

    int totalCount = await featuresQuery.CountAsync(cancellationToken);

    List<Feature> features = await featuresQuery
        .OrderBy(feature => feature.Title)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return (features, totalCount);
  }

  public async Task<Feature?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    return await dbContext.Features
        .FirstOrDefaultAsync(feature => feature.ProjectId == projectId && feature.Slug == slug, cancellationToken);
  }

  public async Task Update(Feature feature, CancellationToken cancellationToken = default)
  {
    dbContext.Features.Update(feature);
    await SaveChanges(feature.Slug, cancellationToken);
  }

  public async Task Delete(Feature feature, CancellationToken cancellationToken = default)
  {
    dbContext.Features.Remove(feature);
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
        postgresException.ConstraintName == "IX_features_project_id_slug")
    {
      throw new DuplicateFeatureSlugException(slug);
    }
  }
}
