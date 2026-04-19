using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Audit.ListDocumentationEvents;

public class ListDocumentationAuditEventsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentationAuditEvents()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id);
    AuditEvent auditEvent = new(
        project.Id,
        documentation.Id,
        AuditEntityType.Documentation,
        documentation.Id,
        documentation.Slug,
        documentation.Title,
        AuditAction.Updated);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeAuditEventRepository auditEventRepository = new()
    {
      DocumentationEvents = [auditEvent]
    };
    ListDocumentationAuditEventsQueryHandler handler = new(
        auditEventRepository,
        documentationRepository,
        projectRepository);

    ListDocumentationAuditEventsResponse response = await handler.Execute(
        project.Slug,
        documentation.Slug,
        new ListDocumentationAuditEventsQuery());

    Assert.Single(response.Items);
    Assert.Equal(project.Id, auditEventRepository.ReceivedProjectId);
    Assert.Equal(documentation.Id, auditEventRepository.ReceivedDocumentationId);
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Documentation CreateDocumentation(Guid projectId)
  {
    return new Documentation(
        projectId,
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
  }
}
