using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Milestones;

public class MilestoneRepository(ProyectoAtlasDbContext dbContext) : IMilestoneRepository
{
  public async Task Add(Milestone milestone, CancellationToken cancellationToken = default)
  {
    await dbContext.Milestones.AddAsync(milestone, cancellationToken);
    await SaveChanges(milestone.Slug, cancellationToken);
  }

  public async Task<(IEnumerable<Milestone> Milestones, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      MilestoneStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    IQueryable<Milestone> milestonesQuery = dbContext.Milestones
        .Where(milestone => milestone.ProjectId == projectId);

    if (!string.IsNullOrWhiteSpace(query))
    {
      string normalizedQuery = $"%{query.Trim()}%";

      milestonesQuery = milestonesQuery.Where(milestone =>
          EF.Functions.ILike(milestone.Title, normalizedQuery) ||
          EF.Functions.ILike(milestone.Summary, normalizedQuery));
    }

    if (status.HasValue)
    {
      milestonesQuery = milestonesQuery.Where(milestone => milestone.Status == status.Value);
    }

    int totalCount = await milestonesQuery.CountAsync(cancellationToken);

    List<Milestone> milestones = await milestonesQuery
        .OrderBy(milestone => milestone.TargetDateUtc.HasValue)
        .ThenBy(milestone => milestone.TargetDateUtc)
        .ThenBy(milestone => milestone.Title)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return (milestones, totalCount);
  }

  public async Task<Milestone?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    return await dbContext.Milestones
        .FirstOrDefaultAsync(milestone => milestone.ProjectId == projectId && milestone.Slug == slug, cancellationToken);
  }

  public async Task Update(Milestone milestone, CancellationToken cancellationToken = default)
  {
    dbContext.Milestones.Update(milestone);
    await SaveChanges(milestone.Slug, cancellationToken);
  }

  public async Task Delete(Milestone milestone, CancellationToken cancellationToken = default)
  {
    dbContext.Milestones.Remove(milestone);
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
        postgresException.ConstraintName == "IX_milestones_project_id_slug")
    {
      throw new DuplicateMilestoneSlugException(slug);
    }
  }
}
