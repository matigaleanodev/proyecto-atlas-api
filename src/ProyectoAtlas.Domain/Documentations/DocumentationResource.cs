namespace ProyectoAtlas.Domain.Documentations;

public class DocumentationResource
{
  private DocumentationResource()
  {
  }

  public DocumentationResource(
      Guid documentationId,
      string title,
      string url,
      DocumentationResourceKind kind)
  {
    if (documentationId == Guid.Empty)
    {
      throw new ArgumentException("Documentation id is required.", nameof(documentationId));
    }

    if (string.IsNullOrWhiteSpace(title))
    {
      throw new InvalidDocumentationResourceException("Documentation resource title is required.");
    }

    if (!Uri.TryCreate(url?.Trim(), UriKind.Absolute, out Uri? resourceUri) ||
        (resourceUri.Scheme != Uri.UriSchemeHttp && resourceUri.Scheme != Uri.UriSchemeHttps))
    {
      throw new InvalidDocumentationResourceException("Documentation resource url must be a valid absolute HTTP or HTTPS url.");
    }

    DateTime now = DateTime.UtcNow;
    string normalizedTitle = title.Trim();
    string normalizedUrl = resourceUri.AbsoluteUri.Trim();

    Id = Guid.NewGuid();
    DocumentationId = documentationId;
    Title = normalizedTitle;
    NormalizedTitle = normalizedTitle.ToLowerInvariant();
    Url = normalizedUrl;
    NormalizedUrl = normalizedUrl.ToLowerInvariant();
    Kind = kind;
    CreatedAtUtc = now;
  }

  public Guid Id { get; private set; }
  public Guid DocumentationId { get; private set; }
  public string Title { get; private set; } = null!;
  public string NormalizedTitle { get; private set; } = null!;
  public string Url { get; private set; } = null!;
  public string NormalizedUrl { get; private set; } = null!;
  public DocumentationResourceKind Kind { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
}
