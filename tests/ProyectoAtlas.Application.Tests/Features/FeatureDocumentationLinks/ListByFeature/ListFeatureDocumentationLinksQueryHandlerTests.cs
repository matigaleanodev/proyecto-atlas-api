using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.FeatureDocumentationLinks.ListByFeature;

public class ListFeatureDocumentationLinksQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnFeatureLinks()
  {
    Project project = CreateProject();
    Feature feature = CreateFeature(project.Id, "Authentication API");
    FeatureDocumentationLink link = new(project.Id, feature.Id, Guid.NewGuid());
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeFeatureRepository featureRepository = new()
    {
      FeatureBySlug = feature
    };
    FakeFeatureDocumentationLinkRepository linkRepository = new()
    {
      FeatureLinks = [link]
    };
    ListFeatureDocumentationLinksQueryHandler handler = new(linkRepository, featureRepository, projectRepository);

    ListFeatureDocumentationLinksResponse result = await handler.Execute(project.Slug, feature.Slug);

    Assert.Single(result.Items);
    Assert.Equal(feature.Id, linkRepository.ReceivedFeatureId);
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Feature CreateFeature(Guid projectId, string title)
  {
    return new Feature(projectId, title, "Expose login endpoints.", FeatureStatus.Planned);
  }
}
