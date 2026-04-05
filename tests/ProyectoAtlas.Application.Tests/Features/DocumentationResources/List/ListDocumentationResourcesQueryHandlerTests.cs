using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationResources.List;

public class ListDocumentationResourcesQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnResources()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    DocumentationResource resource = new(
        documentation.Id,
        "OpenAPI Spec",
        "https://api.example.com/openapi.json",
        DocumentationResourceKind.ApiSpec);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeDocumentationResourceRepository resourceRepository = new()
    {
      Resources = [resource]
    };
    ListDocumentationResourcesQueryHandler handler = new(resourceRepository, documentationRepository, projectRepository);

    ListDocumentationResourcesResponse result = await handler.Execute("proyecto-atlas", documentation.Slug);

    Assert.Single(result.Items);
    Assert.Equal(documentation.Id, resourceRepository.ReceivedDocumentationId);
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
