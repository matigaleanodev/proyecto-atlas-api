using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ProjectRelations.ListIncoming;

public class ListIncomingProjectRelationsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnIncomingRelations()
  {
    Project project = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    ProjectRelation relation = new(Guid.NewGuid(), project.Id, ProjectRelationKind.IntegratesWith);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeProjectRelationRepository relationRepository = new()
    {
      IncomingRelations = [relation]
    };
    ListIncomingProjectRelationsQueryHandler handler = new(relationRepository, projectRepository);

    ListProjectRelationsResponse response = await handler.Execute(project.Slug);

    Assert.Single(response.Items);
    Assert.Equal(project.Id, relationRepository.ReceivedTargetProjectId);
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
