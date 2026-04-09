using ProyectoAtlas.Domain.Common;

namespace ProyectoAtlas.Domain.Milestones;

public class Milestone
{
  private Milestone()
  {
  }

  public Milestone(
      Guid projectId,
      string title,
      string summary,
      MilestoneStatus status,
      DateTime? targetDateUtc = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(summary);

    if (projectId == Guid.Empty)
    {
      throw new ArgumentException("Project id is required.", nameof(projectId));
    }

    DateTime now = DateTime.UtcNow;

    Id = Guid.NewGuid();
    ProjectId = projectId;
    Title = title.Trim();
    Summary = summary.Trim();
    Slug = SlugGenerator.Generate(title.Trim());
    Status = status;
    TargetDateUtc = targetDateUtc;
    CreatedAtUtc = now;
    UpdatedAtUtc = now;
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public string Title { get; private set; } = null!;
  public string Summary { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public MilestoneStatus Status { get; private set; }
  public DateTime? TargetDateUtc { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime UpdatedAtUtc { get; private set; }

  public void Update(
      string? title,
      string? summary,
      MilestoneStatus? status,
      DateTime? targetDateUtc,
      bool clearTargetDate)
  {
    if (!string.IsNullOrWhiteSpace(title))
    {
      Title = title.Trim();
      Slug = SlugGenerator.Generate(title.Trim());
    }

    if (!string.IsNullOrWhiteSpace(summary))
    {
      Summary = summary.Trim();
    }

    if (status.HasValue)
    {
      Status = status.Value;
    }

    if (clearTargetDate)
    {
      TargetDateUtc = null;
    }
    else if (targetDateUtc.HasValue)
    {
      TargetDateUtc = targetDateUtc.Value;
    }

    UpdatedAtUtc = DateTime.UtcNow;
  }
}
