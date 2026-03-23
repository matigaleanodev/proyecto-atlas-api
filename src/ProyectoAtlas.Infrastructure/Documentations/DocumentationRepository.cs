using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Infrastructure.Documentations;

public class DocumentationRepository(ProyectoAtlasDbContext dbContext) : IDocumentationRepository
{
  public async Task Add(Documentation documentation, CancellationToken cancellationToken = default)
  {
    await dbContext.Documentations.AddAsync(documentation, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task Delete(Documentation documentation, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Documentation?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<(IEnumerable<Documentation> Documentations, int TotalCount)> GetPagedList(Guid projectId, int page, int pageSize, string? query = null, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task Update(Documentation documentation, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
