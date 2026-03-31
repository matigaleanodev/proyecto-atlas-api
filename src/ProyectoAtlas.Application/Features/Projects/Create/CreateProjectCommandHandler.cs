using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Create;

public class CreateProjectCommandHandler(IProjectRepository projectRepository)
{

  public async Task<Project> Execute(CreateProjectCommand input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Description);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.RepositoryUrl);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Color);

    if (input.Links is not null)
    {
      if (input.Links.Any(link =>
          string.IsNullOrWhiteSpace(link.Title) ||
          string.IsNullOrWhiteSpace(link.Url) ||
          string.IsNullOrWhiteSpace(link.Description)))
      {
        throw new InvalidProjectLinkItemException("Project links must have a non-empty title, URL, and description.");
      }

      if (input.Links.Any(link => link.SortOrder < 1))
      {
        throw new InvalidProjectLinkItemException("Project links must have a sort order greater than 0.");
      }

      int distinctSortOrders = input.Links
          .Select(link => link.SortOrder)
          .Distinct()
          .Count();

      if (distinctSortOrders != input.Links.Count)
      {
        throw new InvalidProjectLinkItemException("Project links cannot have duplicate sort order values.");
      }
    }

    Project project;

    try
    {
      List<ProjectLinkData>? links = input.Links?
          .Select(link => new ProjectLinkData(
              link.Title,
              link.Url,
              link.Description,
              link.SortOrder,
              link.Kind))
          .ToList();

      project = new(
          input.Title,
          input.Description,
          input.RepositoryUrl,
          input.Color,
          links);


    }
    catch (InvalidProjectLinkListException exception)
    {
      throw new InvalidProjectLinkItemException(exception.Message);
    }

    await projectRepository.Add(project, cancellationToken);

    return project;
  }
}

