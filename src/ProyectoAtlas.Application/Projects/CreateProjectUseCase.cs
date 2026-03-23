using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects;

public class CreateProjectUseCase(IProjectRepository projectRepository)
{

  public async Task<Project> Execute(CreateProjectInput input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Description);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.RepositoryUrl);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Color);

    var project = new Project(
        input.Title,
        input.Description,
        input.RepositoryUrl,
        input.Color);

    await projectRepository.Add(project, cancellationToken);

    return project;
  }
}

