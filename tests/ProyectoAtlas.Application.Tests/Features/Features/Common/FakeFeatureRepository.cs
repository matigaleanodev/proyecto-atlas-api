using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Tests.Features.Features.Common;

internal sealed class FakeFeatureRepository : IFeatureRepository
{
  public Feature? AddedFeature { get; private set; }
  public Feature? UpdatedFeature { get; private set; }
  public Feature? DeletedFeature { get; private set; }
  public Guid ReceivedProjectId { get; private set; }
  public int ReceivedPage { get; private set; }
  public int ReceivedPageSize { get; private set; }
  public string? ReceivedQuery { get; private set; }
  public FeatureStatus? ReceivedStatus { get; private set; }
  public IEnumerable<Feature> PagedFeatures { get; set; } = [];
  public int PagedTotalCount { get; set; }
  public Feature? FeatureBySlug { get; set; }

  public Task Add(Feature feature, CancellationToken cancellationToken = default)
  {
    AddedFeature = feature;
    return Task.CompletedTask;
  }

  public Task<(IEnumerable<Feature> Features, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      FeatureStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    ReceivedPage = page;
    ReceivedPageSize = pageSize;
    ReceivedQuery = query;
    ReceivedStatus = status;

    return Task.FromResult((PagedFeatures, PagedTotalCount));
  }

  public Task<Feature?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    return Task.FromResult(FeatureBySlug);
  }

  public Task Update(Feature feature, CancellationToken cancellationToken = default)
  {
    UpdatedFeature = feature;
    return Task.CompletedTask;
  }

  public Task Delete(Feature feature, CancellationToken cancellationToken = default)
  {
    DeletedFeature = feature;
    return Task.CompletedTask;
  }
}
