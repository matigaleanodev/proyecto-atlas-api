using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Common;

public interface IProjectRepository
{
  Task Add(Project project, CancellationToken cancellationToken = default);

  Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedList(int page, int pageSize, string? query = null, CancellationToken cancellationToken = default);

  Task<Project?> GetBySlug(string slug, CancellationToken cancellationToken = default);

  Task Update(Project project, CancellationToken cancellationToken = default);

  Task Delete(Project project, CancellationToken cancellationToken = default);

}
