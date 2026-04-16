using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Audit.ListDocumentationEvents;

public class ListDocumentationAuditEventsQueryHandler(
    IAuditEventRepository auditEventRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
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

    IReadOnlyCollection<Domain.Audit.AuditEvent> events = await auditEventRepository.GetDocumentationEvents(
        project.Id,
        documentation.Id,
        cancellationToken);

    return new ListDocumentationAuditEventsResponse(events);
  }
}
