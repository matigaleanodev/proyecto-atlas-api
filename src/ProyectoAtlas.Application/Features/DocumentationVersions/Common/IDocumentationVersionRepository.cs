using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationVersions.Common;

public interface IDocumentationVersionRepository
{
  Task<int> GetNextVersionNumber(Guid documentationId, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<DocumentationVersion>> GetList(
      Guid documentationId,
      CancellationToken cancellationToken = default);

  Task<DocumentationVersion?> GetByVersionNumber(
      Guid documentationId,
      int versionNumber,
      CancellationToken cancellationToken = default);
}
