using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects
{
  public class ListProjectsUseCase(IProjectRepository projectRepository)
  {
    public async Task<ListProjectsOutput> Execute(ListProjectsInput input, CancellationToken cancellationToken = default)
    {
      var (page, pageSize, query) = input;
      var (projects, totalCount) = await projectRepository.GetPagedList(page, pageSize, query, cancellationToken);
      var items = projects.ToList();
      var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

      return new ListProjectsOutput(items, page, pageSize, totalPages, totalCount);
    }
  }
}