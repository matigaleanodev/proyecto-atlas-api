using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects;

public interface IProjectRepository
{
    Task Add(Project project, CancellationToken cancellationToken = default);
}
