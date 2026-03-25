using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.GetBySlug;

public class GetProjectBySlugQueryHandler(IProjectRepository projectRepository)
{
  public async Task<Project> Execute(string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

    return project;
  }
}
