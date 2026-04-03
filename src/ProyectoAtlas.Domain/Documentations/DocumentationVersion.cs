namespace ProyectoAtlas.Domain.Documentations;

public class DocumentationVersion
{
  private DocumentationVersion()
  {
  }

  public DocumentationVersion(
      Guid documentationId,
      int versionNumber,
      string title,
      string contentMarkdown,
      DocumentationStatus status)
  {
    if (documentationId == Guid.Empty)
    {
      throw new ArgumentException("Documentation id is required.", nameof(documentationId));
    }

    if (versionNumber < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(versionNumber), "Version number must be greater than 0.");
    }

    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(contentMarkdown);

    Id = Guid.NewGuid();
    DocumentationId = documentationId;
    VersionNumber = versionNumber;
    Title = title;
    ContentMarkdown = contentMarkdown;
    Status = status;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid DocumentationId { get; private set; }
  public int VersionNumber { get; private set; }
  public string Title { get; private set; } = null!;
  public string ContentMarkdown { get; private set; } = null!;
  public DocumentationStatus Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }

  public static DocumentationVersion CreateSnapshot(Documentation documentation, int versionNumber)
  {
    ArgumentNullException.ThrowIfNull(documentation);

    return new DocumentationVersion(
        documentation.Id,
        versionNumber,
        documentation.Title,
        documentation.ContentMarkdown,
        documentation.Status);
  }
}
