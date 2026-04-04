using ProyectoAtlas.Application.Features.DocumentationVersions.List;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationVersions.List;

public class ListDocumentationVersionsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnVersions()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    DocumentationVersion version = new(
        documentation.Id,
        1,
        "Getting Started",
        "# Atlas",
        DocumentationStatus.Draft);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeDocumentationVersionRepository versionRepository = new()
    {
      Versions = [version]
    };
    ListDocumentationVersionsQueryHandler handler = new(versionRepository, documentationRepository, projectRepository);

    ListDocumentationVersionsResponse result = await handler.Execute("proyecto-atlas", documentation.Slug);

    Assert.Single(result.Items);
    Assert.Equal(documentation.Id, versionRepository.ReceivedDocumentationId);
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
