using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Tests;

public class ThrowingProjectRepository(IProjectRepository innerRepository) : IProjectRepository
{
  public async Task Add(Project project, CancellationToken cancellationToken = default)
  {
    await innerRepository.Add(project, cancellationToken);
  }

  public async Task Delete(Project project, CancellationToken cancellationToken = default)
  {
    await innerRepository.Delete(project, cancellationToken);
  }

  public async Task<Project?> GetBySlug(string slug, CancellationToken cancellationToken = default)
  {
    if (slug == "unexpected-project")
    {
      throw new InvalidOperationException("Simulated unknown failure.");
    }

    return await innerRepository.GetBySlug(slug, cancellationToken);
  }

  public async Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedList(
      int page,
      int pageSize,
      string? query = null,
      CancellationToken cancellationToken = default)
  {
    return await innerRepository.GetPagedList(page, pageSize, query, cancellationToken);
  }

  public async Task Update(Project project, CancellationToken cancellationToken = default)
  {
    await innerRepository.Update(project, cancellationToken);
  }
}
