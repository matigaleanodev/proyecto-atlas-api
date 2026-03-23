using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class ListProjectsUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnPagedProjects()
  {
    var projectRepository = new FakeProjectRepository();
    projectRepository.PagedProjects = new[]
    {
      new Project("Proyecto Atlas", "Backend for project documentation", "https://github.com/matigaleanodev/proyecto-atlas-api", "#1E293B"),
      new Project("Atlas Docs", "Documentation explorer", "https://github.com/matigaleanodev/atlas-docs", "#0F172A")
    };
    projectRepository.PagedTotalCount = 3;
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
    projectRepository.PagedProjects = [];
    var useCase = new ListProjectsUseCase(projectRepository);
    var input = new ListProjectsInput(Page: 3, PageSize: 5, Query: "docs");

    await useCase.Execute(input);

    Assert.Equal(input.Page, projectRepository.ReceivedPage);
    Assert.Equal(input.PageSize, projectRepository.ReceivedPageSize);
    Assert.Equal(input.Query, projectRepository.ReceivedQuery);
  }
}
