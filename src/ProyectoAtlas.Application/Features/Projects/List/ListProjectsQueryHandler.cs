using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.List;

public class ListProjectsQueryHandler(IProjectRepository projectRepository)
{
  public async Task<ListProjectsResponse> Execute(ListProjectsQuery input, CancellationToken cancellationToken = default)
  {
    (int page, int pageSize, string? query) = input;
    (IEnumerable<Project>? projects, int totalCount) = await projectRepository.GetPagedList(page, pageSize, query, cancellationToken);
    List<Project> items = [.. projects];
    int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    return new ListProjectsResponse(items, page, pageSize, totalPages, totalCount);
  }
}
