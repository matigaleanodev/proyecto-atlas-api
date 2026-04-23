using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.FeatureDocumentationLinks.Delete;

public class DeleteFeatureDocumentationLinkCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteLink_WhenLinkExists()
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
      LinkById = link
    };
    DeleteFeatureDocumentationLinkCommandHandler handler = new(linkRepository, featureRepository, projectRepository);

    await handler.Execute(project.Slug, feature.Slug, link.Id);

    Assert.Same(link, linkRepository.DeletedLink);
  }

  [Fact]
  public async Task Execute_ShouldThrowFeatureDocumentationLinkNotFoundException_WhenLinkDoesNotExist()
  {
    Project project = CreateProject();
    Feature feature = CreateFeature(project.Id, "Authentication API");
    DeleteFeatureDocumentationLinkCommandHandler handler = new(
        new FakeFeatureDocumentationLinkRepository(),
        new FakeFeatureRepository { FeatureBySlug = feature },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<FeatureDocumentationLinkNotFoundException>(() =>
        handler.Execute(project.Slug, feature.Slug, Guid.NewGuid()));
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
