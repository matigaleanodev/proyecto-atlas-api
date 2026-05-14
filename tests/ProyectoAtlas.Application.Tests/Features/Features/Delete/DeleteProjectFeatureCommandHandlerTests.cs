using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Features.Delete;

public class DeleteProjectFeatureCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteFeature_WhenSlugExists()
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
    DeleteProjectFeatureCommandHandler handler = new(
        featureRepository,
        new FakeFeatureDocumentationLinkRepository(),
        new FakeMilestoneFeatureLinkRepository(),
        projectRepository);

    await handler.Execute("proyecto-atlas", feature.Slug);

    Assert.Same(feature, featureRepository.DeletedFeature);
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
    DeleteProjectFeatureCommandHandler handler = new(
        new FakeFeatureRepository(),
        new FakeFeatureDocumentationLinkRepository(),
        new FakeMilestoneFeatureLinkRepository(),
        projectRepository);

    await Assert.ThrowsAsync<FeatureNotFoundException>(() => handler.Execute("proyecto-atlas", "missing-feature"));
  }

  [Fact]
  public async Task Execute_ShouldThrowFeatureDeleteBlockedException_WhenDocumentationLinksExist()
  {
    Project project = CreateProject();
    Feature feature = new(project.Id, "Authentication API", "Expose login endpoints.", FeatureStatus.Planned);
    DeleteProjectFeatureCommandHandler handler = new(
        new FakeFeatureRepository { FeatureBySlug = feature },
        new FakeFeatureDocumentationLinkRepository
        {
          FeatureLinks =
          [
            new FeatureDocumentationLink(project.Id, feature.Id, Guid.NewGuid())
          ]
        },
        new FakeMilestoneFeatureLinkRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<FeatureDeleteBlockedException>(() => handler.Execute("proyecto-atlas", feature.Slug));
  }

  [Fact]
  public async Task Execute_ShouldThrowFeatureDeleteBlockedException_WhenMilestoneLinksExist()
  {
    Project project = CreateProject();
    Feature feature = new(project.Id, "Authentication API", "Expose login endpoints.", FeatureStatus.Planned);
    DeleteProjectFeatureCommandHandler handler = new(
        new FakeFeatureRepository { FeatureBySlug = feature },
        new FakeFeatureDocumentationLinkRepository(),
        new FakeMilestoneFeatureLinkRepository
        {
          FeatureLinks =
          [
            new Domain.Milestones.MilestoneFeatureLink(project.Id, Guid.NewGuid(), feature.Id)
          ]
        },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<FeatureDeleteBlockedException>(() => handler.Execute("proyecto-atlas", feature.Slug));
  }

  private static Project CreateProject()
  {
    return new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }
}
