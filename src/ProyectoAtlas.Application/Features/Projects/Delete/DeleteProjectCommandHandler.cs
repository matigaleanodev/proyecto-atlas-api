using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Delete;

public class DeleteProjectCommandHandler(IProjectRepository projectRepository)
{
  public async Task Execute(string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

    await projectRepository.Delete(project, cancellationToken);
  }
}
