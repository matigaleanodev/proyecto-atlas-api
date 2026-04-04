using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationVersions.Common;

internal sealed class FakeDocumentationVersionRepository : IDocumentationVersionRepository
{
  public int NextVersionNumber { get; set; } = 1;
  public IReadOnlyCollection<DocumentationVersion> Versions { get; set; } = [];
  public DocumentationVersion? VersionByNumber { get; set; }
  public Guid ReceivedDocumentationId { get; private set; }
  public int ReceivedVersionNumber { get; private set; }

  public Task<int> GetNextVersionNumber(Guid documentationId, CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    return Task.FromResult(NextVersionNumber);
  }

  public Task<IReadOnlyCollection<DocumentationVersion>> GetList(Guid documentationId, CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    return Task.FromResult(Versions);
  }

  public Task<DocumentationVersion?> GetByVersionNumber(Guid documentationId, int versionNumber, CancellationToken cancellationToken = default)
  {
    ReceivedDocumentationId = documentationId;
    ReceivedVersionNumber = versionNumber;
    return Task.FromResult(VersionByNumber);
  }
}
