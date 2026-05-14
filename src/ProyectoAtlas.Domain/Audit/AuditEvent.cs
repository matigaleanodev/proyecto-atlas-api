namespace ProyectoAtlas.Domain.Audit;

public class AuditEvent
{
  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public Guid? DocumentationId { get; private set; }
  public AuditEntityType EntityType { get; private set; }
  public Guid EntityId { get; private set; }
  public string EntitySlug { get; private set; }
  public string EntityTitle { get; private set; }
  public AuditAction Action { get; private set; }
  public DateTime OccurredAtUtc { get; private set; }

  private AuditEvent()
  {
    EntitySlug = string.Empty;
    EntityTitle = string.Empty;
  }

  public AuditEvent(
      Guid projectId,
      Guid? documentationId,
      AuditEntityType entityType,
      Guid entityId,
      string entitySlug,
      string entityTitle,
      AuditAction action)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(entitySlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(entityTitle);

    if (projectId == Guid.Empty)
    {
      throw new ArgumentException("Project id cannot be empty.", nameof(projectId));
    }

    if (entityId == Guid.Empty)
    {
      throw new ArgumentException("Entity id cannot be empty.", nameof(entityId));
    }

    if (entityType == AuditEntityType.Project && documentationId.HasValue)
    {
      throw new ArgumentException("Project audit events cannot reference a documentation id.", nameof(documentationId));
    }

    if (entityType == AuditEntityType.Documentation && !documentationId.HasValue)
    {
      throw new ArgumentException("Documentation audit events must reference a documentation id.", nameof(documentationId));
    }

    Id = Guid.NewGuid();
    ProjectId = projectId;
    DocumentationId = documentationId;
    EntityType = entityType;
    EntityId = entityId;
    EntitySlug = entitySlug.Trim();
    EntityTitle = entityTitle.Trim();
    Action = action;
    OccurredAtUtc = DateTime.UtcNow;
  }
}
