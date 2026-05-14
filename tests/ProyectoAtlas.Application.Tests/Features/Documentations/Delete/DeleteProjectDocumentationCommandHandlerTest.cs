using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Documentations.Delete;

public class DeleteProjectDocumentationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteDocumentation_WhenDocumentationExists()
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
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project,
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation,
    };
    FakeAuditEventRepository auditEventRepository = new();
    DeleteProjectDocumentationCommandHandler useCase = new(
        documentationRepository,
        new FakeDocumentationRelationRepository(),
        new FakeFeatureDocumentationLinkRepository(),
        auditEventRepository,
        projectRepository);

    await useCase.Execute("proyecto-atlas", "getting-started");

    Assert.Same(documentation, documentationRepository.DeletedDocumentation);
    Assert.NotNull(auditEventRepository.AddedAuditEvent);
    Assert.Equal(Domain.Audit.AuditAction.Deleted, auditEventRepository.AddedAuditEvent.Action);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    DeleteProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeDocumentationRelationRepository(),
        new FakeFeatureDocumentationLinkRepository(),
        new FakeAuditEventRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
        useCase.Execute("missing-project", "getting-started"));
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
      ProjectBySlug = project,
    };
    DeleteProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeDocumentationRelationRepository(),
        new FakeFeatureDocumentationLinkRepository(),
        new FakeAuditEventRepository(),
        projectRepository);

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() =>
        useCase.Execute("proyecto-atlas", "missing-doc"));
  }

  [Theory]
  [InlineData(null, "getting-started")]
  [InlineData("", "getting-started")]
  [InlineData("   ", "getting-started")]
  [InlineData("proyecto-atlas", null)]
  [InlineData("proyecto-atlas", "")]
  [InlineData("proyecto-atlas", "   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenSlugInputIsInvalid(
      string? projectSlug,
      string? slug)
  {
    DeleteProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeDocumentationRelationRepository(),
        new FakeFeatureDocumentationLinkRepository(),
        new FakeAuditEventRepository(),
        new FakeProjectRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        useCase.Execute(projectSlug!, slug!));
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationDeleteBlockedException_WhenOutgoingRelationsExist()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id);
    DeleteProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeDocumentationRelationRepository
        {
          OutgoingRelations =
          [
            new DocumentationRelation(project.Id, documentation.Id, Guid.NewGuid(), DocumentationRelationKind.RelatedTo)
          ]
        },
        new FakeFeatureDocumentationLinkRepository(),
        new FakeAuditEventRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationDeleteBlockedException>(() =>
        useCase.Execute("proyecto-atlas", documentation.Slug));
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationDeleteBlockedException_WhenFeatureLinksExist()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id);
    DeleteProjectDocumentationCommandHandler useCase = new(
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeDocumentationRelationRepository(),
        new FakeFeatureDocumentationLinkRepository
        {
          DocumentationLinks =
          [
            new Domain.Features.FeatureDocumentationLink(project.Id, Guid.NewGuid(), documentation.Id)
          ]
        },
        new FakeAuditEventRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationDeleteBlockedException>(() =>
        useCase.Execute("proyecto-atlas", documentation.Slug));
  }

  private static Project CreateProject()
  {
    return new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Documentation CreateDocumentation(Guid projectId)
  {
    return new(
        projectId,
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
  }
}
