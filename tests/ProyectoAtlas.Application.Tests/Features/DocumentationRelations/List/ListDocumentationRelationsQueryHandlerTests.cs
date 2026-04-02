using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationRelations.List;

public class ListDocumentationRelationsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnOutgoingRelations()
  {
    Project project = CreateProject();
    Documentation sourceDocumentation = CreateDocumentation(project.Id, "Getting Started");
    DocumentationRelation relation = new(project.Id, sourceDocumentation.Id, Guid.NewGuid(), DocumentationRelationKind.Implements);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = sourceDocumentation
    };
    FakeDocumentationRelationRepository relationRepository = new()
    {
      OutgoingRelations = [relation]
    };
    ListDocumentationRelationsQueryHandler handler = new(relationRepository, documentationRepository, projectRepository);

    ListDocumentationRelationsResponse result = await handler.Execute("proyecto-atlas", sourceDocumentation.Slug);

    Assert.Single(result.Items);
    Assert.Equal(sourceDocumentation.Id, relationRepository.ReceivedSourceDocumentationId);
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
