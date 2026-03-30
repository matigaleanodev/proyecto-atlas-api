namespace ProyectoAtlas.Domain.Projects;

public class ProjectLink
{
  private ProjectLink()
  {
  }

  public ProjectLink(Guid projectId, string title, int sortOrder, string url, ProjectLinkKind kind, string description)
  {

    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(url);
    ArgumentException.ThrowIfNullOrWhiteSpace(description);

    if (projectId == Guid.Empty)
    {
      throw new ArgumentException("Project id is required.", nameof(projectId));
    }

    if (sortOrder < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(sortOrder), "Sort order must be greater than 0.");
    }

    bool isValidUrl = Uri.TryCreate(url.Trim(), UriKind.Absolute, out Uri? uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    if (!isValidUrl)
    {
      throw new ArgumentException("Invalid URL format.", nameof(url));
    }

    Id = Guid.NewGuid();
    ProjectId = projectId;
    Title = title.Trim();
    SortOrder = sortOrder;
    Url = url.Trim();
    Kind = kind;
    Description = description.Trim();
  }

  public Guid Id { get; private set; }
  public Guid ProjectId { get; private set; }
  public string Title { get; private set; } = null!;
  public int SortOrder { get; private set; }
  public string Description { get; private set; } = null!;
  public string Url { get; private set; } = null!;
  public ProjectLinkKind Kind { get; private set; }


}
