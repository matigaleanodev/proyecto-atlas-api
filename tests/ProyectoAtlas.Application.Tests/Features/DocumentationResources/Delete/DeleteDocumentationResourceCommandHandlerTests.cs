using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationResources.Delete;

public class DeleteDocumentationResourceCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteResource_WhenResourceExists()
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
      ResourceById = resource
    };
    DeleteDocumentationResourceCommandHandler handler = new(resourceRepository, documentationRepository, projectRepository);

    await handler.Execute("proyecto-atlas", documentation.Slug, resource.Id);

    Assert.Same(resource, resourceRepository.DeletedResource);
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationResourceNotFoundException_WhenResourceDoesNotExist()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    DeleteDocumentationResourceCommandHandler handler = new(
        new FakeDocumentationResourceRepository(),
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationResourceNotFoundException>(() =>
        handler.Execute("proyecto-atlas", documentation.Slug, Guid.NewGuid()));
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
