using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationRelations.Delete;

public class DeleteDocumentationRelationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteRelation_WhenRelationExists()
  {
    Project project = CreateProject();
    Documentation sourceDocumentation = CreateDocumentation(project.Id, "Getting Started");
    DocumentationRelation relation = new(project.Id, sourceDocumentation.Id, Guid.NewGuid(), DocumentationRelationKind.RelatedTo);
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
      RelationById = relation
    };
    DeleteDocumentationRelationCommandHandler handler = new(relationRepository, documentationRepository, projectRepository);

    await handler.Execute("proyecto-atlas", sourceDocumentation.Slug, relation.Id);

    Assert.Same(relation, relationRepository.DeletedRelation);
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationRelationNotFoundException_WhenRelationDoesNotExist()
  {
    Project project = CreateProject();
    Documentation sourceDocumentation = CreateDocumentation(project.Id, "Getting Started");
    DeleteDocumentationRelationCommandHandler handler = new(
        new FakeDocumentationRelationRepository(),
        new FakeDocumentationRepository { DocumentationBySlug = sourceDocumentation },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationRelationNotFoundException>(() =>
        handler.Execute("proyecto-atlas", sourceDocumentation.Slug, Guid.NewGuid()));
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
