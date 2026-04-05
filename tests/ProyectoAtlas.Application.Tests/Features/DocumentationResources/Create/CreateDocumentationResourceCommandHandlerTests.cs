using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationResources.Create;

public class CreateDocumentationResourceCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnResource()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeDocumentationResourceRepository resourceRepository = new();
    CreateDocumentationResourceCommandHandler handler = new(resourceRepository, documentationRepository, projectRepository);
    CreateDocumentationResourceCommand input = new("OpenAPI Spec", "https://api.example.com/openapi.json", DocumentationResourceKind.ApiSpec);

    DocumentationResource result = await handler.Execute("proyecto-atlas", documentation.Slug, input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Url, result.Url);
    Assert.Equal(input.Kind, result.Kind);
    Assert.Equal(documentation.Id, result.DocumentationId);
    Assert.Same(result, resourceRepository.AddedResource);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationResourceItemException_WhenUrlIsInvalid()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    CreateDocumentationResourceCommandHandler handler = new(
        new FakeDocumentationResourceRepository(),
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<InvalidDocumentationResourceItemException>(() =>
        handler.Execute(
            "proyecto-atlas",
            documentation.Slug,
            new CreateDocumentationResourceCommand("OpenAPI Spec", "nota-url", DocumentationResourceKind.ApiSpec)));
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
