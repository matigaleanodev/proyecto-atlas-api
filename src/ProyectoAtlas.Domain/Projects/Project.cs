using ProyectoAtlas.Domain.Common;

namespace ProyectoAtlas.Domain.Projects;

public class Project
{
  private readonly List<ProjectLink> _links = [];

  private Project()
  {
  }

  public Project(string title, string description, string repositoryUrl, string color, IReadOnlyCollection<ProjectLinkData>? links = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(description);
    ArgumentException.ThrowIfNullOrWhiteSpace(repositoryUrl);
    ArgumentException.ThrowIfNullOrWhiteSpace(color);

    if (!CanAssignLinks(links))
    {
      throw new InvalidProjectLinkListException(
          "Project links must have non-empty title, description, valid URL, and unique sort order.");
    }



    DateTime now = DateTime.UtcNow;



    Id = Guid.NewGuid();
    Title = title.Trim();
    Description = description.Trim();
    RepositoryUrl = repositoryUrl.Trim();
    Color = color.Trim();
    Slug = SlugGenerator.Generate(title.Trim());
    CreatedAtUtc = now;
    UpdatedAtUtc = now;

    if (links is { Count: > 0 })
    {
      ReplaceLinksInternal(links);
    }
  }

  public Guid Id { get; private set; }
  public string Title { get; private set; } = null!;
  public string Description { get; private set; } = null!;
  public string RepositoryUrl { get; private set; } = null!;
  public string Color { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime UpdatedAtUtc { get; private set; }
  public IReadOnlyCollection<ProjectLink> Links => _links.AsReadOnly();


  public void Update(string? title, string? description, string? repositoryUrl, string? color)
  {
    if (!string.IsNullOrWhiteSpace(title))
    {
      Title = title.Trim();
      Slug = SlugGenerator.Generate(title.Trim());
    }

    if (!string.IsNullOrWhiteSpace(description))
    {
      Description = description.Trim();
    }

    if (!string.IsNullOrWhiteSpace(repositoryUrl))
    {
      RepositoryUrl = repositoryUrl.Trim();
    }

    if (!string.IsNullOrWhiteSpace(color))
    {
      Color = color.Trim();
    }

    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void ReplaceLinks(IReadOnlyCollection<ProjectLinkData> links)
  {
    if (!CanAssignLinks(links))
    {
      throw new InvalidProjectLinkListException(
          "Project links must have non-empty title, description, valid URL, and unique sort order.");
    }

    ReplaceLinksInternal(links);
    UpdatedAtUtc = DateTime.UtcNow;
  }


  private void ReplaceLinksInternal(IEnumerable<ProjectLinkData> links)
  {
    _links.Clear();

    foreach (ProjectLinkData linkData in links.OrderBy(link => link.SortOrder))
    {
      _links.Add(new ProjectLink(
          Id,
          linkData.Title,
          linkData.Url,
          linkData.Description,
          linkData.SortOrder,
          linkData.Kind));
    }

  }

  private static bool CanAssignLinks(IReadOnlyCollection<ProjectLinkData>? links)
  {
    IReadOnlyCollection<ProjectLinkData> linkList = links ?? [];

    if (linkList.Count == 0)
    {
      return true;
    }

    bool hasValidData = linkList.All(link =>
        !string.IsNullOrWhiteSpace(link.Title) &&
        !string.IsNullOrWhiteSpace(link.Url) &&
        !string.IsNullOrWhiteSpace(link.Description) &&
        link.SortOrder >= 1 &&
        Uri.TryCreate(link.Url.Trim(), UriKind.Absolute, out _));

    if (!hasValidData)
    {
      return false;
    }

    int distinctSortOrders = linkList
        .Select(link => link.SortOrder)
        .Distinct()
        .Count();

    return distinctSortOrders == linkList.Count;
  }

}



