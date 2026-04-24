namespace ProyectoAtlas.Domain.Milestones;

public class MilestoneFeatureLink
{
  private MilestoneFeatureLink()
  {
  }

  public MilestoneFeatureLink(Guid projectId, Guid milestoneId, Guid featureId)
  {
    if (projectId == Guid.Empty)
    {
      throw new ArgumentException("Project id is required.", nameof(projectId));
    }

    if (milestoneId == Guid.Empty)
    {
      throw new ArgumentException("Milestone id is required.", nameof(milestoneId));
    }

    if (featureId == Guid.Empty)
    {
      throw new ArgumentException("Feature id is required.", nameof(featureId));
    }

    Id = Guid.NewGuid();
    ProjectId = projectId;
    MilestoneId = milestoneId;
    FeatureId = featureId;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public Guid MilestoneId { get; private set; }
  public Guid FeatureId { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
}
