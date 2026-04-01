using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Features.Features.Common;

public interface IFeatureRepository
{
  Task Add(Feature feature, CancellationToken cancellationToken = default);

  Task<(IEnumerable<Feature> Features, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      FeatureStatus? status = null,
      CancellationToken cancellationToken = default);

  Task<Feature?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default);

  Task Update(Feature feature, CancellationToken cancellationToken = default);

  Task Delete(Feature feature, CancellationToken cancellationToken = default);
}
