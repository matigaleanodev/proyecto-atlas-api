using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Features.List;

public class ListProjectFeaturesQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnPagedFeatures()
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
    FakeFeatureRepository featureRepository = new()
    {
      PagedFeatures =
      [
        new Feature(project.Id, "Authentication API", "Expose login endpoints.", FeatureStatus.Planned),
        new Feature(project.Id, "Project Features", "Expose feature management.", FeatureStatus.InProgress)
      ],
      PagedTotalCount = 3
    };
    ListProjectFeaturesQueryHandler handler = new(featureRepository, projectRepository);
    ListProjectFeaturesQuery input = new(Page: 2, PageSize: 2, Query: "api", Status: FeatureStatus.Planned);

    ListProjectFeaturesResponse result = await handler.Execute("proyecto-atlas", input);

    Assert.Equal(input.Page, result.Page);
    Assert.Equal(input.PageSize, result.PageSize);
    Assert.Equal(3, result.TotalItems);
    Assert.Equal(2, result.TotalPages);
    Assert.Equal(2, result.Items.Count);
    Assert.Equal(project.Id, featureRepository.ReceivedProjectId);
    Assert.Equal(input.Query, featureRepository.ReceivedQuery);
    Assert.Equal(input.Status, featureRepository.ReceivedStatus);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    ListProjectFeaturesQueryHandler handler = new(new FakeFeatureRepository(), new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => handler.Execute("missing-project", new ListProjectFeaturesQuery()));
  }
}
