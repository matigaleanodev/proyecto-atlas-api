namespace ProyectoAtlas.Domain.Projects;

public class ProjectRelation
{
  private ProjectRelation()
  {
  }

  public ProjectRelation(
      Guid sourceProjectId,
      Guid targetProjectId,
      ProjectRelationKind kind)
  {
    if (sourceProjectId == Guid.Empty)
    {
      throw new ArgumentException("Source project id is required.", nameof(sourceProjectId));
    }

    if (targetProjectId == Guid.Empty)
    {
      throw new ArgumentException("Target project id is required.", nameof(targetProjectId));
    }

    if (sourceProjectId == targetProjectId)
    {
      throw new InvalidProjectRelationException("Project relation cannot reference the same project.");
    }

    Id = Guid.NewGuid();
    SourceProjectId = sourceProjectId;
    TargetProjectId = targetProjectId;
    Kind = kind;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid SourceProjectId { get; private set; }
  public Guid TargetProjectId { get; private set; }
  public ProjectRelationKind Kind { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
}
