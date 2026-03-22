using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Projects;

public class ProjectRepository(ProyectoAtlasDbContext dbContext) : IProjectRepository
{
  public async Task Add(Project project, CancellationToken cancellationToken = default)
  {
    await dbContext.Projects.AddAsync(project, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

  }
}