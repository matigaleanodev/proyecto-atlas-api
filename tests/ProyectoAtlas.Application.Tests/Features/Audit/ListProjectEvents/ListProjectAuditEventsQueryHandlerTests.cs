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
    ListProjectAuditEventsQuery query = new(
        AuditEntityType.Project,
        AuditAction.Created,
        new DateTime(2026, 05, 10, 0, 0, 0, DateTimeKind.Utc),
        new DateTime(2026, 05, 11, 0, 0, 0, DateTimeKind.Utc),
        10);

    ListProjectAuditEventsResponse response = await handler.Execute(project.Slug, query);

    Assert.Single(response.Items);
    Assert.Equal(project.Id, auditEventRepository.ReceivedProjectId);
    Assert.Equal(query, auditEventRepository.ReceivedFilters);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidAuditEventQueryException_WhenLimitIsNotPositive()
  {
    Project project = CreateProject();
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    ListProjectAuditEventsQueryHandler handler = new(new FakeAuditEventRepository(), projectRepository);

    await Assert.ThrowsAsync<InvalidAuditEventQueryException>(() =>
        handler.Execute(project.Slug, new ListProjectAuditEventsQuery(Limit: 0)));
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidAuditEventQueryException_WhenOccurredFromUtcIsGreaterThanOccurredToUtc()
  {
    Project project = CreateProject();
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    ListProjectAuditEventsQueryHandler handler = new(new FakeAuditEventRepository(), projectRepository);

    await Assert.ThrowsAsync<InvalidAuditEventQueryException>(() =>
        handler.Execute(
            project.Slug,
            new ListProjectAuditEventsQuery(
                OccurredFromUtc: new DateTime(2026, 05, 11, 0, 0, 0, DateTimeKind.Utc),
                OccurredToUtc: new DateTime(2026, 05, 10, 0, 0, 0, DateTimeKind.Utc))));
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
