using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ProjectRelations.List;

public class ListProjectRelationsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnOutgoingRelations()
  {
    Project project = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    ProjectRelation relation = new(project.Id, Guid.NewGuid(), ProjectRelationKind.IntegratesWith);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeProjectRelationRepository relationRepository = new()
    {
      OutgoingRelations = [relation]
    };
    ListProjectRelationsQueryHandler handler = new(relationRepository, projectRepository);

    ListProjectRelationsResponse result = await handler.Execute(project.Slug);

    Assert.Single(result.Items);
    Assert.Equal(project.Id, relationRepository.ReceivedSourceProjectId);
  }

  private static Project CreateProject(string title, string repositoryUrl)
  {
    return new Project(
        title,
        "Backend for project documentation based on markdown",
        repositoryUrl,
        "#1E293B");
  }
}
