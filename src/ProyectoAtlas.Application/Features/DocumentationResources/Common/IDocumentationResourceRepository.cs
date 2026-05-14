using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Common;

public interface IDocumentationResourceRepository
{
  Task Add(DocumentationResource resource, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<DocumentationResource>> GetList(
      Guid documentationId,
      DocumentationResourceKind? kind = null,
      CancellationToken cancellationToken = default);
  Task<DocumentationResource?> GetById(Guid documentationId, Guid resourceId, CancellationToken cancellationToken = default);
  Task Delete(DocumentationResource resource, CancellationToken cancellationToken = default);
}
