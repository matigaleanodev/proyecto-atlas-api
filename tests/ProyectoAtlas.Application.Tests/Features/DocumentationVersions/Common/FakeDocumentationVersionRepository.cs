using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationVersions.Common;

internal sealed class FakeDocumentationVersionRepository : IDocumentationVersionRepository
{
  public int NextVersionNumber { get; set; } = 1;
  public IReadOnlyCollection<DocumentationVersion> Versions { get; set; } = [];
  public DocumentationVersion? VersionByNumber { get; set; }

  public Task<int> GetNextVersionNumber(Guid documentationId, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(NextVersionNumber);
  }

  public Task<IReadOnlyCollection<DocumentationVersion>> GetList(Guid documentationId, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(Versions);
  }

  public Task<DocumentationVersion?> GetByVersionNumber(Guid documentationId, int versionNumber, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(VersionByNumber);
  }
}
