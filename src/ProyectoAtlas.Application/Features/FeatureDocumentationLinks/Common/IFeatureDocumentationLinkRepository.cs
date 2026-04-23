using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.Common;

public interface IFeatureDocumentationLinkRepository
{
  Task Add(FeatureDocumentationLink link, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FeatureDocumentationLink>> GetByFeatureId(Guid featureId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FeatureDocumentationLink>> GetByDocumentationId(Guid documentationId, CancellationToken cancellationToken = default);
  Task<FeatureDocumentationLink?> GetById(Guid featureId, Guid linkId, CancellationToken cancellationToken = default);
  Task Delete(FeatureDocumentationLink link, CancellationToken cancellationToken = default);
}
