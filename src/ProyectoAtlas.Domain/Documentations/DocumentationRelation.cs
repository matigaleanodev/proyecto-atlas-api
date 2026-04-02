namespace ProyectoAtlas.Domain.Documentations;

public class DocumentationRelation
{
  private DocumentationRelation()
  {
  }

  public DocumentationRelation(
      Guid projectId,
      Guid sourceDocumentationId,
      Guid targetDocumentationId,
      DocumentationRelationKind kind)
  {
    if (projectId == Guid.Empty)
    {
      throw new ArgumentException("Project id is required.", nameof(projectId));
    }

    if (sourceDocumentationId == Guid.Empty)
    {
      throw new ArgumentException("Source documentation id is required.", nameof(sourceDocumentationId));
    }

    if (targetDocumentationId == Guid.Empty)
    {
      throw new ArgumentException("Target documentation id is required.", nameof(targetDocumentationId));
    }

    if (sourceDocumentationId == targetDocumentationId)
    {
      throw new InvalidDocumentationRelationException("Documentation relation cannot reference the same documentation.");
    }

    Id = Guid.NewGuid();
    ProjectId = projectId;
    SourceDocumentationId = sourceDocumentationId;
    TargetDocumentationId = targetDocumentationId;
    Kind = kind;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public Guid SourceDocumentationId { get; private set; }
  public Guid TargetDocumentationId { get; private set; }
  public DocumentationRelationKind Kind { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
}
