using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Projects;

public class ProjectRepository(ProyectoAtlasDbContext dbContext) : IProjectRepository
{
  public async Task Add(Project project, CancellationToken cancellationToken = default)
  {
    await dbContext.Projects.AddAsync(project, cancellationToken);
    await SaveChanges(project.Slug, cancellationToken);
  }

  public async Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedList(
      int page,
      int pageSize,
      string? query = null,
      CancellationToken cancellationToken = default)
  {
    IQueryable<Project> projectsQuery = dbContext.Projects;

    if (!string.IsNullOrWhiteSpace(query))
    {
      string normalizedQuery = $"%{query.Trim()}%";

      projectsQuery = projectsQuery.Where(project =>
          EF.Functions.ILike(project.Title, normalizedQuery) ||
          EF.Functions.ILike(project.Description, normalizedQuery));
    }

    int totalCount = await projectsQuery.CountAsync(cancellationToken);

    List<Project> projects = await projectsQuery
        .OrderByDescending(project => project.CreatedAtUtc)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return (projects, totalCount);
  }

  public async Task<Project?> GetBySlug(string slug, CancellationToken cancellationToken = default)
  {
    return await dbContext.Projects.FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
  }

  public async Task Update(Project project, CancellationToken cancellationToken = default)
  {
    dbContext.Projects.Update(project);
    await SaveChanges(project.Slug, cancellationToken);
  }

  public async Task Delete(Project project, CancellationToken cancellationToken = default)
  {
    dbContext.Projects.Remove(project);
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
        postgresException.ConstraintName == "IX_projects_slug")
    {
      throw new DuplicateProjectSlugException(slug);
    }
  }
}
