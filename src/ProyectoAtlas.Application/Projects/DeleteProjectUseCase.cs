using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects;

public class DeleteProjectUseCase(IProjectRepository projectRepository)
{
  public async Task<Project> Execute(string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    var project = await projectRepository.GetBySlug(slug, cancellationToken) ?? throw new KeyNotFoundException($"Project with slug '{slug}' not found.");

    await projectRepository.Delete(project, cancellationToken);

    return project;
  }
}