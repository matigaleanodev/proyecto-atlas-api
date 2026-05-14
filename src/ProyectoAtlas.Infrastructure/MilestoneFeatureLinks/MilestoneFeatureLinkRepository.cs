using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.MilestoneFeatureLinks;

public class MilestoneFeatureLinkRepository(ProyectoAtlasDbContext dbContext) : IMilestoneFeatureLinkRepository
{
  public async Task Add(MilestoneFeatureLink link, CancellationToken cancellationToken = default)
  {
    await dbContext.MilestoneFeatureLinks.AddAsync(link, cancellationToken);
    await SaveChanges(cancellationToken);
  }

  public async Task<IReadOnlyCollection<MilestoneFeatureLink>> GetByMilestoneId(Guid milestoneId, CancellationToken cancellationToken = default)
  {
    return await dbContext.MilestoneFeatureLinks
        .Where(link => link.MilestoneId == milestoneId)
        .OrderBy(link => link.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<MilestoneFeatureLink>> GetByFeatureId(Guid featureId, CancellationToken cancellationToken = default)
  {
    return await dbContext.MilestoneFeatureLinks
        .Where(link => link.FeatureId == featureId)
        .OrderBy(link => link.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<MilestoneFeatureLink?> GetById(Guid milestoneId, Guid linkId, CancellationToken cancellationToken = default)
  {
    return await dbContext.MilestoneFeatureLinks
        .FirstOrDefaultAsync(link => link.MilestoneId == milestoneId && link.Id == linkId, cancellationToken);
  }

  public async Task Delete(MilestoneFeatureLink link, CancellationToken cancellationToken = default)
  {
    dbContext.MilestoneFeatureLinks.Remove(link);
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
      throw new DuplicateMilestoneFeatureLinkException();
    }
  }
}
