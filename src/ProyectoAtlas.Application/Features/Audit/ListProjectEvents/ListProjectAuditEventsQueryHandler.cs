using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Audit.ListProjectEvents;

public class ListProjectAuditEventsQueryHandler(
    IAuditEventRepository auditEventRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListProjectAuditEventsResponse> Execute(
      string projectSlug,
      ListProjectAuditEventsQuery query,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    IReadOnlyCollection<Domain.Audit.AuditEvent> events = await auditEventRepository.GetProjectEvents(
        project.Id,
        cancellationToken);

    return new ListProjectAuditEventsResponse(events);
  }
}
