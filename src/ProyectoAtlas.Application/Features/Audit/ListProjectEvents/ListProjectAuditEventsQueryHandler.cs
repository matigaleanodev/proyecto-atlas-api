using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Audit.ListProjectEvents;

public class ListProjectAuditEventsQueryHandler(
    IAuditEventRepository auditEventRepository,
    IProjectRepository projectRepository)
{
  private const int MaxLimit = 100;

  public async Task<ListProjectAuditEventsResponse> Execute(
      string projectSlug,
      ListProjectAuditEventsQuery query,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    ValidateQuery(query);

    IReadOnlyCollection<Domain.Audit.AuditEvent> events = await auditEventRepository.GetProjectEvents(
        project.Id,
        query,
        cancellationToken);

    return new ListProjectAuditEventsResponse(events);
  }

  private static void ValidateQuery(ListProjectAuditEventsQuery query)
  {
    if (query.Limit is <= 0)
    {
      throw new InvalidAuditEventQueryException("Audit event limit must be greater than 0.");
    }

    if (query.Limit is > MaxLimit)
    {
      throw new InvalidAuditEventQueryException($"Audit event limit cannot be greater than {MaxLimit}.");
    }

    if (query.OccurredFromUtc.HasValue &&
        query.OccurredToUtc.HasValue &&
        query.OccurredFromUtc.Value > query.OccurredToUtc.Value)
    {
      throw new InvalidAuditEventQueryException("Audit event occurredFromUtc cannot be greater than occurredToUtc.");
    }
  }
}
