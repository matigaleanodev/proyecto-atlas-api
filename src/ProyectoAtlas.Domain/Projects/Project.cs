using ProyectoAtlas.Domain.Common;

namespace ProyectoAtlas.Domain.Projects;

public class Project
{
  private Project()
  {
  }

  public Project(string title, string description, string repositoryUrl, string color)
  {
    DateTime now = DateTime.UtcNow;

    Id = Guid.NewGuid();
    Title = title;
    Description = description;
    RepositoryUrl = repositoryUrl;
    Color = color;
    Slug = SlugGenerator.Generate(title);
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


  public void Update(string? title, string? description, string? repositoryUrl, string? color)
  {
    if (!string.IsNullOrWhiteSpace(title))
    {
      Title = title;
      Slug = SlugGenerator.Generate(title);
    }

    if (!string.IsNullOrWhiteSpace(description))
    {
      Description = description;
    }

    if (!string.IsNullOrWhiteSpace(repositoryUrl))
    {
      RepositoryUrl = repositoryUrl;
    }

    if (!string.IsNullOrWhiteSpace(color))
    {
      Color = color;
    }

    UpdatedAtUtc = DateTime.UtcNow;
  }


}



