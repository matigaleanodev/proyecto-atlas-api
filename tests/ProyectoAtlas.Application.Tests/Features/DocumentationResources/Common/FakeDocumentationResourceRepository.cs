using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationResources.Common;

internal sealed class FakeDocumentationResourceRepository : IDocumentationResourceRepository
{
  public DocumentationResource? AddedResource { get; private set; }
  public DocumentationResource? DeletedResource { get; private set; }
  public Guid ReceivedDocumentationId { get; private set; }
  public Guid ReceivedResourceId { get; private set; }
  public IReadOnlyCollection<DocumentationResource> Resources { get; set; } = [];
  public DocumentationResource? ResourceById { get; set; }

  public Task Add(DocumentationResource resource, CancellationToken cancellationToken = default)
  {
    AddedResource = resource;
    return Task.CompletedTask;
  }

  public Task<IReadOnlyCollection<DocumentationResource>> GetList(Guid documentationId, CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    return Task.FromResult(Resources);
  }

  public Task<DocumentationResource?> GetById(Guid documentationId, Guid resourceId, CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    ReceivedResourceId = resourceId;
    return Task.FromResult(ResourceById);
  }

  public Task Delete(DocumentationResource resource, CancellationToken cancellationToken = default)
  {
    DeletedResource = resource;
    return Task.CompletedTask;
  }
}
