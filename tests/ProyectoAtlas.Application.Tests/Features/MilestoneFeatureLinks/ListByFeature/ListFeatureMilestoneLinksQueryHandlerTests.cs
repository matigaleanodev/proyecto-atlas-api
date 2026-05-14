using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.MilestoneFeatureLinks.ListByFeature;

public class ListFeatureMilestoneLinksQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnFeatureLinks()
  {
    Project project = CreateProject();
    Feature feature = CreateFeature(project.Id, "Authentication API");
    MilestoneFeatureLink link = new(project.Id, Guid.NewGuid(), feature.Id);
    FakeMilestoneFeatureLinkRepository linkRepository = new()
    {
      FeatureLinks = [link]
    };
    ListFeatureMilestoneLinksQueryHandler handler = new(
        linkRepository,
        new FakeFeatureRepository { FeatureBySlug = feature },
        new FakeProjectRepository { ProjectBySlug = project });

    ListFeatureMilestoneLinksResponse result = await handler.Execute(project.Slug, feature.Slug);

    Assert.Single(result.Items);
    Assert.Equal(feature.Id, linkRepository.ReceivedFeatureId);
  }

  private static Project CreateProject()
  {
    return new Project("Proyecto Atlas", "Backend for project documentation based on markdown", "https://github.com/matigaleanodev/proyecto-atlas-api", "#1E293B");
  }

  private static Feature CreateFeature(Guid projectId, string title)
  {
    return new Feature(projectId, title, "Expose login endpoints.", FeatureStatus.Planned);
  }
}
