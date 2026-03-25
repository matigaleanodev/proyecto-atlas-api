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

    Project project = new(
        input.Title,
        input.Description,
        input.RepositoryUrl,
        input.Color);

    await projectRepository.Add(project, cancellationToken);

    return project;
  }
}

