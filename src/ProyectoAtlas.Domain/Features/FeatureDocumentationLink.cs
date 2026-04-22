namespace ProyectoAtlas.Domain.Features;

public class FeatureDocumentationLink
{
  private FeatureDocumentationLink()
  {
  }

  public FeatureDocumentationLink(Guid projectId, Guid featureId, Guid documentationId)
  {
    if (projectId == Guid.Empty)
    {
      throw new ArgumentException("Project id is required.", nameof(projectId));
    }

    if (featureId == Guid.Empty)
    {
      throw new ArgumentException("Feature id is required.", nameof(featureId));
    }

    if (documentationId == Guid.Empty)
    {
      throw new ArgumentException("Documentation id is required.", nameof(documentationId));
    }

    Id = Guid.NewGuid();
    ProjectId = projectId;
    FeatureId = featureId;
    DocumentationId = documentationId;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public Guid FeatureId { get; private set; }
  public Guid DocumentationId { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
}
