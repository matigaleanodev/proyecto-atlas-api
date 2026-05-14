using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.FeatureDocumentationLinks.ListByDocumentation;

public class ListDocumentationFeatureLinksQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentationLinks()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    FeatureDocumentationLink link = new(project.Id, Guid.NewGuid(), documentation.Id);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeFeatureDocumentationLinkRepository linkRepository = new()
    {
      DocumentationLinks = [link]
    };
    ListDocumentationFeatureLinksQueryHandler handler = new(
        linkRepository,
        documentationRepository,
        projectRepository);

    ListDocumentationFeatureLinksResponse result = await handler.Execute(project.Slug, documentation.Slug);

    Assert.Single(result.Items);
    Assert.Equal(documentation.Id, linkRepository.ReceivedDocumentationId);
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Documentation CreateDocumentation(Guid projectId, string title)
  {
    return new Documentation(
        projectId,
        title,
        "# Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
  }
}
