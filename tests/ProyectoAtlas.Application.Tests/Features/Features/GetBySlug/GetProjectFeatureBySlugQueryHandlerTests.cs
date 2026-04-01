using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Features.GetBySlug;

public class GetProjectFeatureBySlugQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnFeature_WhenSlugExists()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Feature feature = new(project.Id, "Authentication API", "Expose login endpoints.", FeatureStatus.Planned);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeFeatureRepository featureRepository = new()
    {
      FeatureBySlug = feature
    };
    GetProjectFeatureBySlugQueryHandler handler = new(featureRepository, projectRepository);

    Feature result = await handler.Execute("proyecto-atlas", feature.Slug);

    Assert.Same(feature, result);
  }

  [Fact]
  public async Task Execute_ShouldThrowFeatureNotFoundException_WhenFeatureDoesNotExist()
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
    GetProjectFeatureBySlugQueryHandler handler = new(new FakeFeatureRepository(), projectRepository);

    await Assert.ThrowsAsync<FeatureNotFoundException>(() => handler.Execute("proyecto-atlas", "missing-feature"));
  }
}
