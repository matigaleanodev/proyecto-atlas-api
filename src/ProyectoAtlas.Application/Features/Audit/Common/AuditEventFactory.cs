using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Audit.Common;

public static class AuditEventFactory
{
  public static AuditEvent ForProject(Project project, AuditAction action)
  {
    return new AuditEvent(
        project.Id,
        documentationId: null,
        AuditEntityType.Project,
        project.Id,
        project.Slug,
        project.Title,
        action);
  }

  public static AuditEvent ForDocumentation(Documentation documentation, AuditAction action)
  {
    return new AuditEvent(
        documentation.ProjectId,
        documentation.Id,
        AuditEntityType.Documentation,
        documentation.Id,
        documentation.Slug,
        documentation.Title,
        action);
  }
}
