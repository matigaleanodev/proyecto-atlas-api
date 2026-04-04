using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.DocumentationVersions;

public class DocumentationVersionRepository(ProyectoAtlasDbContext dbContext) : IDocumentationVersionRepository
{
  public async Task<int> GetNextVersionNumber(Guid documentationId, CancellationToken cancellationToken = default)
  {
    int? latestVersionNumber = await dbContext.DocumentationVersions
        .Where(version => version.DocumentationId == documentationId)
        .MaxAsync(version => (int?)version.VersionNumber, cancellationToken);

    return latestVersionNumber.GetValueOrDefault() + 1;
  }

  public async Task<IReadOnlyCollection<DocumentationVersion>> GetList(
      Guid documentationId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.DocumentationVersions
        .Where(version => version.DocumentationId == documentationId)
        .OrderByDescending(version => version.VersionNumber)
        .ToListAsync(cancellationToken);
  }

  public async Task<DocumentationVersion?> GetByVersionNumber(
      Guid documentationId,
      int versionNumber,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.DocumentationVersions
        .FirstOrDefaultAsync(
            version => version.DocumentationId == documentationId && version.VersionNumber == versionNumber,
            cancellationToken);
  }
}
