using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Features.Update;

public class UpdateProjectFeatureCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldUpdateFeature_WhenSlugExists()
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
    UpdateProjectFeatureCommandHandler handler = new(featureRepository, projectRepository);
    UpdateProjectFeatureCommand input = new("Authentication API", "Expose login and refresh endpoints.", FeatureStatus.InProgress);

    Feature result = await handler.Execute("proyecto-atlas", feature.Slug, input);

    Assert.Equal(input.Summary, result.Summary);
    Assert.Equal(input.Status, result.Status);
    Assert.Same(feature, featureRepository.UpdatedFeature);
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
    UpdateProjectFeatureCommandHandler handler = new(new FakeFeatureRepository(), projectRepository);

    await Assert.ThrowsAsync<FeatureNotFoundException>(() =>
        handler.Execute("proyecto-atlas", "missing-feature", new UpdateProjectFeatureCommand(null, null, FeatureStatus.Done)));
  }
}
