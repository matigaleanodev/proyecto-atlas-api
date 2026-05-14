using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Audit.ListDocumentationEvents;

public class ListDocumentationAuditEventsQueryHandler(
    IAuditEventRepository auditEventRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  private const int MaxLimit = 100;

  public async Task<ListDocumentationAuditEventsResponse> Execute(
      string projectSlug,
      string documentationSlug,
      ListDocumentationAuditEventsQuery query,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(documentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, documentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, documentationSlug);

    ValidateQuery(query);

    IReadOnlyCollection<Domain.Audit.AuditEvent> events = await auditEventRepository.GetDocumentationEvents(
        project.Id,
        documentation.Id,
        query,
        cancellationToken);

    return new ListDocumentationAuditEventsResponse(events);
  }

  private static void ValidateQuery(ListDocumentationAuditEventsQuery query)
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
