using System.Text.RegularExpressions;

namespace ProyectoAtlas.Domain.Documentations;

public partial class Documentation
{
  [GeneratedRegex(@"^ADR-\d{3,}\s.+$")]
  private static partial Regex AdrTitleRegex();

  private Documentation()
  {
  }

  public Documentation(Guid projectId, string title, string contentMarkdown, int sortOrder, DocumentationKind kind, DocumentationStatus status)
  {
    if (kind == DocumentationKind.Decision && !ValidateAdrTitle(title))
    {
      throw new ArgumentException("ADR documentation title must follow the format 'ADR-XXX Title'", nameof(title));
    }

    DateTime now = DateTime.UtcNow;

    Id = Guid.NewGuid();
    ProjectId = projectId;
    Title = title;
    Slug = title.Trim().ToLowerInvariant().Replace(' ', '-');
    ContentMarkdown = contentMarkdown;
    SortOrder = sortOrder;
    Kind = kind;
    Status = status;
    CreatedAtUtc = now;
    UpdatedAtUtc = now;
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public string Title { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public string ContentMarkdown { get; private set; } = null!;
  public int SortOrder { get; private set; }
  public DocumentationKind Kind { get; private set; }
  public DocumentationStatus Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime UpdatedAtUtc { get; private set; }

  public void Update(string? title, string? contentMarkdown, int? sortOrder, DocumentationStatus? status)
  {
    if (!string.IsNullOrWhiteSpace(title))
    {
      if (Kind == DocumentationKind.Decision && !ValidateAdrTitle(title))
      {
        throw new ArgumentException(
            "ADR documentation title must follow the format 'ADR-XXX Title'",
            nameof(title));
      }

      Title = title;
      Slug = title.Trim().ToLowerInvariant().Replace(' ', '-');
    }


    if (!string.IsNullOrWhiteSpace(contentMarkdown))
    {
      ContentMarkdown = contentMarkdown;
    }

    if (sortOrder.HasValue)
    {
      SortOrder = sortOrder.Value;
    }

    if (status.HasValue)
    {
      Status = status.Value;
    }

    UpdatedAtUtc = DateTime.UtcNow;
  }


  private static bool ValidateAdrTitle(string title)
  {
    if (string.IsNullOrWhiteSpace(title))
    {
      return false;
    }

    return AdrTitleRegex().IsMatch(title);
  }

}
