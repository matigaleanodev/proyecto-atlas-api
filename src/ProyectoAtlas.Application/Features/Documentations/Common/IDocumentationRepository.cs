using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public interface IDocumentationRepository
{
  Task Add(Documentation documentation, CancellationToken cancellationToken = default);

  Task<(IEnumerable<Documentation> Documentations, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      DocumentationKind? kind = null,
      DocumentationStatus? status = null,
      DocumentationArea? area = null,
      CancellationToken cancellationToken = default
    );

  Task<Documentation?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default);

  Task Update(Documentation documentation, CancellationToken cancellationToken = default);

  Task Delete(Documentation documentation, CancellationToken cancellationToken = default);
}
