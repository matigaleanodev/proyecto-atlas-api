using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationRelations.ListIncoming;

public class ListIncomingDocumentationRelationsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnIncomingRelations()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    DocumentationRelation relation = new(project.Id, Guid.NewGuid(), documentation.Id, DocumentationRelationKind.Implements);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeDocumentationRelationRepository relationRepository = new()
    {
      IncomingRelations = [relation]
    };
    ListIncomingDocumentationRelationsQueryHandler handler = new(
        relationRepository,
        documentationRepository,
        projectRepository);

    ListDocumentationRelationsResponse response = await handler.Execute(
        project.Slug,
        documentation.Slug);

    Assert.Single(response.Items);
    Assert.Equal(documentation.Id, relationRepository.ReceivedTargetDocumentationId);
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
