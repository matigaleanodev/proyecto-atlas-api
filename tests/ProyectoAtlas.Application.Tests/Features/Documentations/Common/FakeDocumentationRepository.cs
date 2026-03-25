using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Tests.Features.Documentations.Common;

internal sealed class FakeDocumentationRepository : IDocumentationRepository
{
  public Documentation? AddedDocumentation { get; private set; }
  public Documentation? UpdatedDocumentation { get; private set; }
  public Documentation? DeletedDocumentation { get; private set; }
  public Guid ReceivedProjectId { get; private set; }
  public int ReceivedPage { get; private set; }
  public int ReceivedPageSize { get; private set; }
  public string? ReceivedQuery { get; private set; }
  public DocumentationKind? ReceivedKind { get; private set; }
  public DocumentationStatus? ReceivedStatus { get; private set; }
  public IEnumerable<Documentation> PagedDocumentations { get; set; } = [];
  public int PagedTotalCount { get; set; }
  public Documentation? DocumentationBySlug { get; set; }

  public Task Add(Documentation documentation, CancellationToken cancellationToken = default)
  {
    AddedDocumentation = documentation;
    return Task.CompletedTask;
  }

  public Task<(IEnumerable<Documentation> Documentations, int TotalCount)> GetPagedList(
      Guid projectId,
      int page,
      int pageSize,
      string? query = null,
      DocumentationKind? kind = null,
      DocumentationStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    ReceivedPage = page;
    ReceivedPageSize = pageSize;
    ReceivedQuery = query;
    ReceivedKind = kind;
    ReceivedStatus = status;

    return Task.FromResult((PagedDocumentations, PagedTotalCount));
  }

  public Task<Documentation?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
  {
    ReceivedProjectId = projectId;
    return Task.FromResult(DocumentationBySlug);
  }

  public Task Update(Documentation documentation, CancellationToken cancellationToken = default)
  {
    UpdatedDocumentation = documentation;
    return Task.CompletedTask;
  }

  public Task Delete(Documentation documentation, CancellationToken cancellationToken = default)
  {
    DeletedDocumentation = documentation;
    return Task.CompletedTask;
  }
}
