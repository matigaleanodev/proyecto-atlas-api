using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Audit.ListProjectEvents;

public class ListProjectAuditEventsQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnProjectAuditEvents()
  {
    Project project = CreateProject();
    AuditEvent auditEvent = new(
        project.Id,
        documentationId: null,
        AuditEntityType.Project,
        project.Id,
        project.Slug,
        project.Title,
        AuditAction.Created);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeAuditEventRepository auditEventRepository = new()
    {
      ProjectEvents = [auditEvent]
    };
    ListProjectAuditEventsQueryHandler handler = new(auditEventRepository, projectRepository);

    ListProjectAuditEventsResponse response = await handler.Execute(project.Slug, new ListProjectAuditEventsQuery());

    Assert.Single(response.Items);
    Assert.Equal(project.Id, auditEventRepository.ReceivedProjectId);
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }
}
