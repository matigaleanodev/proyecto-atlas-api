using ProyectoAtlas.Domain.Projects;
namespace ProyectoAtlas.Application.Features.Projects.Update;

public class UpdateProjectCommandHandler(IProjectRepository projectRepository)
{
  public async Task<Project> Execute(string slug, UpdateProjectCommand input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

    project.Update(input.Title, input.Description, input.RepositoryUrl, input.Color);

    await projectRepository.Update(project, cancellationToken);

    return project;
  }
}
