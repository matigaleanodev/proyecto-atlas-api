using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Delete;

public class DeleteProjectCommandHandler(
    IProjectRepository projectRepository,
    IAuditEventRepository auditEventRepository)
{
  public async Task Execute(string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

    await auditEventRepository.Add(AuditEventFactory.ForProject(project, Domain.Audit.AuditAction.Deleted), cancellationToken);
    await projectRepository.Delete(project, cancellationToken);
  }
}
