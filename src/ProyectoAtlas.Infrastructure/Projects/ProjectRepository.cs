using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Projects;

public class ProjectRepository(ProyectoAtlasDbContext dbContext) : IProjectRepository
{
  public async Task Add(Project project, CancellationToken cancellationToken = default)
  {
    await dbContext.Projects.AddAsync(project, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
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
      var normalizedQuery = $"%{query.Trim()}%";

      projectsQuery = projectsQuery.Where(project =>
          EF.Functions.ILike(project.Title, normalizedQuery) ||
          EF.Functions.ILike(project.Description, normalizedQuery));
    }

    var totalCount = await projectsQuery.CountAsync(cancellationToken);

    var projects = await projectsQuery
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
    await dbContext.SaveChangesAsync(cancellationToken);
  }

}
