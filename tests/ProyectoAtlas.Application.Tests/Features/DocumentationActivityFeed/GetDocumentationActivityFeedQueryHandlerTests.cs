using ProyectoAtlas.Application.Features.DocumentationActivityFeed;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationActivityFeed;

public class GetDocumentationActivityFeedQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentationActivityFeed()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    Documentation documentation = new(
        project.Id,
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeDocumentationActivityFeedRepository activityFeedRepository = new()
    {
      Items =
      [
        new DocumentationActivityFeedItem(
            DocumentationActivityFeedItemType.VersionCreated,
            DateTime.UtcNow,
            VersionNumber: 1)
      ]
    };
    GetDocumentationActivityFeedQueryHandler handler = new(
        activityFeedRepository,
        documentationRepository,
        projectRepository);

    GetDocumentationActivityFeedResponse response = await handler.Execute(project.Slug, documentation.Slug);

    Assert.Single(response.Items);
    Assert.Equal(documentation.Id, activityFeedRepository.ReceivedDocumentationId);
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationNotFoundException_WhenDocumentationDoesNotExist()
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
    GetDocumentationActivityFeedQueryHandler handler = new(
        new FakeDocumentationActivityFeedRepository(),
        new FakeDocumentationRepository(),
        projectRepository);

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() => handler.Execute(project.Slug, "missing-doc"));
  }
}
