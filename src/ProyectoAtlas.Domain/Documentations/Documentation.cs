using System.Text.RegularExpressions;
using ProyectoAtlas.Domain.Common;

namespace ProyectoAtlas.Domain.Documentations;

public partial class Documentation
{
  private readonly List<DocumentationTag> _tags = [];
  private readonly List<DocumentationFaqItem> _faqItems = [];

  private Documentation()
  {
  }

  public Documentation(
    Guid projectId,
    string title,
    string contentMarkdown,
    int sortOrder,
    DocumentationKind kind,
    DocumentationStatus status,
    DocumentationArea area,
    IReadOnlyCollection<DocumentationTagData>? tags = null,
    IReadOnlyCollection<DocumentationFaqItemData>? faqItems = null)
  {
    Id = Guid.NewGuid();

    if (kind == DocumentationKind.Decision && !ValidateAdrTitle(title))
    {
      throw new InvalidDocumentationTitleException(
          "ADR documentation title must follow the format 'ADR-XXX Title'.");
    }

    if (!CanAssignFaqItems(kind, faqItems))
    {
      throw new InvalidDocumentationFaqListException(
          "FAQ documentation must have at least one FAQ item, and non-FAQ documentation cannot include FAQ items.");
    }



    if (!CanAssignTags(tags))
    {
      throw new InvalidDocumentationTagListException(
          "Documentation cannot have duplicate tags or empty tag names.");
    }

    DateTime now = DateTime.UtcNow;

    ProjectId = projectId;
    Title = title;
    Slug = SlugGenerator.Generate(title);
    ContentMarkdown = contentMarkdown;
    SortOrder = sortOrder;
    Kind = kind;
    Status = status;
    Area = area;
    CreatedAtUtc = now;
    UpdatedAtUtc = now;

    if (tags is { Count: > 0 })
    {
      ReplaceTagsInternal(tags);
    }

    if (kind == DocumentationKind.FAQ)
    {
      ReplaceFaqItemsInternal(faqItems!);
    }
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public string Title { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public string ContentMarkdown { get; private set; } = null!;
  public int SortOrder { get; private set; }
  public DocumentationKind Kind { get; private set; }
  public DocumentationStatus Status { get; private set; }
  public DocumentationArea Area { get; private set; }
  public IReadOnlyCollection<DocumentationTag> Tags => _tags.AsReadOnly();
  public IReadOnlyCollection<DocumentationFaqItem> FaqItems => _faqItems.AsReadOnly();
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime UpdatedAtUtc { get; private set; }

  public void Update(string? title, string? contentMarkdown, int? sortOrder, DocumentationStatus? status)
  {
    if (!string.IsNullOrWhiteSpace(title))
    {
      if (Kind == DocumentationKind.Decision && !ValidateAdrTitle(title))
      {
        throw new InvalidDocumentationTitleException(
            "ADR documentation title must follow the format 'ADR-XXX Title'.");
      }

      Title = title;
      Slug = SlugGenerator.Generate(title);
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

  public void ReplaceTags(IReadOnlyCollection<DocumentationTagData> tags)
  {
    if (!CanAssignTags(tags))
    {
      throw new InvalidDocumentationTagListException(
          "Documentation cannot have duplicate tags or empty tag names.");
    }

    ReplaceTagsInternal(tags);
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void ReplaceFaqItems(IReadOnlyCollection<DocumentationFaqItemData> faqItems)
  {
    if (Kind != DocumentationKind.FAQ)
    {
      throw new InvalidDocumentationFaqListException(
          "Only FAQ documentation can include FAQ items.");
    }

    if (!CanAssignFaqItems(Kind, faqItems))
    {
      throw new InvalidDocumentationFaqListException(
          "FAQ documentation must have at least one FAQ item.");
    }

    ReplaceFaqItemsInternal(faqItems);
    UpdatedAtUtc = DateTime.UtcNow;
  }

  [GeneratedRegex(@"^ADR-\d{3,}\s.+$")]
  private static partial Regex AdrTitleRegex();


  private static bool ValidateAdrTitle(string title)
  {
    if (string.IsNullOrWhiteSpace(title))
    {
      return false;
    }

    return AdrTitleRegex().IsMatch(title);
  }

  private static bool CanAssignFaqItems(DocumentationKind kind, IReadOnlyCollection<DocumentationFaqItemData>? faqItems)
  {
    IReadOnlyCollection<DocumentationFaqItemData> validFaqItems = faqItems ?? [];
    bool hasFaqItems = validFaqItems.Count > 0;

    if (kind != DocumentationKind.FAQ)
    {
      return !hasFaqItems;
    }

    if (!hasFaqItems)
    {
      return false;
    }

    return validFaqItems
        .Select(item => item.SortOrder)
        .Distinct()
        .Count() == validFaqItems.Count;
  }

  private static bool CanAssignTags(IReadOnlyCollection<DocumentationTagData>? tags)
  {
    IReadOnlyCollection<DocumentationTagData> validTags = tags ?? [];
    if (validTags.Count == 0)
    {
      return true;
    }

    return validTags
        .Where(tag => !string.IsNullOrWhiteSpace(tag.Name))
        .Select(tag => tag.Name.Trim().ToLowerInvariant())
        .Distinct()
        .Count() == validTags.Count;
  }

  private void ReplaceFaqItemsInternal(IReadOnlyCollection<DocumentationFaqItemData> faqItems)
  {
    _faqItems.Clear();

    foreach (DocumentationFaqItemData faqItem in faqItems.OrderBy(item => item.SortOrder))
    {
      _faqItems.Add(new DocumentationFaqItem(
          Id,
          faqItem.Question,
          faqItem.Answer,
          faqItem.SortOrder));
    }
  }

  private void ReplaceTagsInternal(IReadOnlyCollection<DocumentationTagData> tags)
  {
    _tags.Clear();

    foreach (DocumentationTagData tag in tags.OrderBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase))
    {
      _tags.Add(new DocumentationTag(Id, tag.Name));
    }
  }
}
