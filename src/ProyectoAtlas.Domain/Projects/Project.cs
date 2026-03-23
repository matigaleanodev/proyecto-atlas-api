namespace ProyectoAtlas.Domain.Projects;

public class Project
{
  private Project()
  {
  }

  public Project(string title, string description, string repositoryUrl, string color)
  {
    var now = DateTime.UtcNow;

    Id = Guid.NewGuid();
    Title = title;
    Description = description;
    RepositoryUrl = repositoryUrl;
    Color = color;
    Slug = title.Trim().ToLowerInvariant().Replace(' ', '-');
    CreatedAtUtc = now;
    UpdatedAtUtc = now;
  }

  public Guid Id { get; private set; }
  public string Title { get; private set; } = null!;
  public string Description { get; private set; } = null!;
  public string RepositoryUrl { get; private set; } = null!;
  public string Color { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime UpdatedAtUtc { get; private set; }
}
