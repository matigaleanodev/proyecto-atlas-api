using ProyectoAtlas.Domain.Projects;
namespace ProyectoAtlas.Application.Features.Projects.Update;

public class UpdateProjectCommandHandler(IProjectRepository projectRepository)
{
  public async Task<Project> Execute(string slug, UpdateProjectCommand input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

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

      int distinctLinksCount = input.Links
          .Select(link => link.SortOrder)
          .Distinct()
          .Count();

      if (distinctLinksCount != input.Links.Count)
      {
        throw new InvalidProjectLinkItemException("Project links cannot have duplicate sort order values.");
      }
    }

    try
    {
      project.Update(input.Title, input.Description, input.RepositoryUrl, input.Color);

      if (input.Links is not null)
      {
        IReadOnlyCollection<ProjectLinkData> links = [.. input.Links
            .Select(link => new ProjectLinkData(
                link.Title,
                link.Url,
                link.Description,
                link.SortOrder,
                link.Kind))];

        project.ReplaceLinks(links);
      }
    }
    catch (InvalidProjectLinkListException exception)
    {
      throw new InvalidProjectLinkItemException(exception.Message);
    }

    await projectRepository.Update(project, cancellationToken);

    return project;
  }
}
