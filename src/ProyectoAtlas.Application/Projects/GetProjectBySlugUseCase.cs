using ProyectoAtlas.Domain.Projects;
namespace ProyectoAtlas.Application.Projects;

public class GetProjectBySlugUseCase(IProjectRepository projectRepository)
{
  public async Task<Project> Execute(string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

    return project;
  }
}
