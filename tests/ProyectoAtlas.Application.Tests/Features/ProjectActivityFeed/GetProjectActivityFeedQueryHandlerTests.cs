using ProyectoAtlas.Application.Features.ProjectActivityFeed;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ProjectActivityFeed;

public class GetProjectActivityFeedQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnProjectActivityFeed()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeProjectActivityFeedRepository activityFeedRepository = new()
    {
      Items =
      [
        new ProjectActivityFeedItem(
            ProjectActivityFeedItemType.FeatureCreated,
            DateTime.UtcNow,
            "authentication-api",
            "Authentication API")
      ]
    };
    GetProjectActivityFeedQueryHandler handler = new(activityFeedRepository, projectRepository);

    GetProjectActivityFeedResponse response = await handler.Execute(project.Slug);

    Assert.Single(response.Items);
    Assert.Equal(project.Id, activityFeedRepository.ReceivedProjectId);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    GetProjectActivityFeedQueryHandler handler = new(new FakeProjectActivityFeedRepository(), new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => handler.Execute("missing-project"));
  }
}
