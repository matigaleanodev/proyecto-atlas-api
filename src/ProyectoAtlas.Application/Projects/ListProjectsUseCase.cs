using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects;

public class ListProjectsUseCase(IProjectRepository projectRepository)
{
  public async Task<ListProjectsOutput> Execute(ListProjectsInput input, CancellationToken cancellationToken = default)
  {
    (int page, int pageSize, string? query) = input;
    (IEnumerable<Project>? projects, int totalCount) = await projectRepository.GetPagedList(page, pageSize, query, cancellationToken);
    List<Project> items = projects.ToList();
    int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    return new ListProjectsOutput(items, page, pageSize, totalPages, totalCount);
  }
}
