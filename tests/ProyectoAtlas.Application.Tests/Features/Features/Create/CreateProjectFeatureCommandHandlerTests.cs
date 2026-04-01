using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Features.Create;

public class CreateProjectFeatureCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnFeature()
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
    FakeFeatureRepository featureRepository = new();
    CreateProjectFeatureCommandHandler handler = new(featureRepository, projectRepository);
    CreateProjectFeatureCommand input = new("Authentication API", "Expose login endpoints.", FeatureStatus.Planned);

    Feature result = await handler.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Summary, result.Summary);
    Assert.Equal(input.Status, result.Status);
    Assert.Equal(project.Id, result.ProjectId);
    Assert.Same(result, featureRepository.AddedFeature);
  }

  [Fact]
  public async Task Execute_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
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
    CreateProjectFeatureCommandHandler handler = new(new FakeFeatureRepository(), projectRepository);
    CreateProjectFeatureCommand input = new("Autenticación API / Inicial", "Expose login endpoints.", FeatureStatus.Planned);

    Feature result = await handler.Execute("proyecto-atlas", input);

    Assert.Equal("autenticacion-api-inicial", result.Slug);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    CreateProjectFeatureCommandHandler handler = new(new FakeFeatureRepository(), new FakeProjectRepository());
    CreateProjectFeatureCommand input = new("Authentication API", "Expose login endpoints.", FeatureStatus.Planned);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => handler.Execute("missing-project", input));
  }
}
