using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class ListProjectsUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnPagedProjects()
  {
    var projectRepository = new FakeProjectRepository();
    var useCase = new ListProjectsUseCase(projectRepository);
    var input = new ListProjectsInput(Page: 2, PageSize: 2, Query: "atlas");

    var result = await useCase.Execute(input);

    Assert.Equal(input.Page, result.Page);
    Assert.Equal(input.PageSize, result.PageSize);
    Assert.Equal(3, result.TotalItems);
    Assert.Equal(2, result.TotalPages);
    Assert.Equal(2, result.Items.Count);
    Assert.Equal("Proyecto Atlas", result.Items.First().Title);
  }

  [Fact]
  public async Task Execute_ShouldPassPagingArgumentsToRepository()
  {
    var projectRepository = new FakeProjectRepository();
    var useCase = new ListProjectsUseCase(projectRepository);
    var input = new ListProjectsInput(Page: 3, PageSize: 5, Query: "docs");

    await useCase.Execute(input);

    Assert.Equal(input.Page, projectRepository.ReceivedPage);
    Assert.Equal(input.PageSize, projectRepository.ReceivedPageSize);
    Assert.Equal(input.Query, projectRepository.ReceivedQuery);
  }

  private sealed class FakeProjectRepository : IProjectRepository
  {
    public int ReceivedPage { get; private set; }
    public int ReceivedPageSize { get; private set; }
    public string? ReceivedQuery { get; private set; }

    public Task Add(Project project, CancellationToken cancellationToken = default)
    {
      throw new NotSupportedException();
    }

    public Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedList(
        int page,
        int pageSize,
        string? query = null,
        CancellationToken cancellationToken = default)
    {
      ReceivedPage = page;
      ReceivedPageSize = pageSize;
      ReceivedQuery = query;

      var projects = new[]
      {
        new Project("Proyecto Atlas", "Backend for project documentation", "https://github.com/matigaleanodev/proyecto-atlas-api", "#1E293B"),
        new Project("Atlas Docs", "Documentation explorer", "https://github.com/matigaleanodev/atlas-docs", "#0F172A")
      };

      return Task.FromResult<(IEnumerable<Project> Projects, int TotalCount)>((projects, 3));
    }
  }
}
