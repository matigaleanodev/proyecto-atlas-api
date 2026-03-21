namespace ProyectoAtlas.Domain.Projects;

public class Project
{
  private Project()
  {
  }

  public Project(string title, string description, string repositoryUrl, string color)
  {
    Id = Guid.NewGuid();
    Title = title;
    Description = description;
    RepositoryUrl = repositoryUrl;
    Color = color;
  }

  public Guid Id { get; private set; }
  public string Title { get; private set; } = null!;
  public string Description { get; private set; } = null!;
  public string RepositoryUrl { get; private set; } = null!;
  public string Color { get; private set; } = null!;
}
