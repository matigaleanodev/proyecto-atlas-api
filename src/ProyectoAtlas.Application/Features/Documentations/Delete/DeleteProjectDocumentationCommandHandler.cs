using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.Delete;

public class DeleteProjectDocumentationCommandHandler(
    IDocumentationRepository documentationRepository,
    IAuditEventRepository auditEventRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(string projectSlug, string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, slug);

    await auditEventRepository.Add(AuditEventFactory.ForDocumentation(documentation, Domain.Audit.AuditAction.Deleted), cancellationToken);
    await documentationRepository.Delete(documentation, cancellationToken);
  }
}
