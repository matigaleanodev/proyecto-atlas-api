using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Tests.Features.FeatureDocumentationLinks.Common;

internal sealed class FakeFeatureDocumentationLinkRepository : IFeatureDocumentationLinkRepository
{
  public FeatureDocumentationLink? AddedLink { get; private set; }
  public FeatureDocumentationLink? DeletedLink { get; private set; }
  public Guid ReceivedFeatureId { get; private set; }
  public Guid ReceivedDocumentationId { get; private set; }
  public Guid ReceivedLinkId { get; private set; }
  public IReadOnlyCollection<FeatureDocumentationLink> FeatureLinks { get; set; } = [];
  public IReadOnlyCollection<FeatureDocumentationLink> DocumentationLinks { get; set; } = [];
  public FeatureDocumentationLink? LinkById { get; set; }

  public Task Add(FeatureDocumentationLink link, CancellationToken cancellationToken = default)
  {
    AddedLink = link;
    return Task.CompletedTask;
  }

  public Task<IReadOnlyCollection<FeatureDocumentationLink>> GetByFeatureId(
      Guid featureId,
      CancellationToken cancellationToken = default)
  {
    ReceivedFeatureId = featureId;
    return Task.FromResult(FeatureLinks);
  }

  public Task<IReadOnlyCollection<FeatureDocumentationLink>> GetByDocumentationId(
      Guid documentationId,
      CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    return Task.FromResult(DocumentationLinks);
  }

  public Task<FeatureDocumentationLink?> GetById(
      Guid featureId,
      Guid linkId,
      CancellationToken cancellationToken = default)
  {
    ReceivedFeatureId = featureId;
    ReceivedLinkId = linkId;
    return Task.FromResult(LinkById);
  }

  public Task Delete(FeatureDocumentationLink link, CancellationToken cancellationToken = default)
  {
    DeletedLink = link;
    return Task.CompletedTask;
  }
}
