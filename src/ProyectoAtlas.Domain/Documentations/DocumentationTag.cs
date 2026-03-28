using System.Text.RegularExpressions;
namespace ProyectoAtlas.Domain.Documentations;

public partial class DocumentationTag
{

  private DocumentationTag()
  {
  }

  public DocumentationTag(Guid documentationId, string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);

    if (documentationId == Guid.Empty)
    {
      throw new ArgumentException("Documentation id is required.", nameof(documentationId));
    }

    string canonicalName = GenerateCanonicalName(name);

    Id = Guid.NewGuid();
    DocumentationId = documentationId;
    Name = canonicalName;
    CanonicalName = canonicalName;
    NormalizedName = NormalizeName(canonicalName);
  }

  public Guid Id { get; private set; }
  public Guid DocumentationId { get; private set; }
  public string Name { get; private set; } = null!;
  public string NormalizedName { get; private set; } = null!;
  public string CanonicalName { get; private set; } = null!;


  private static string NormalizeName(string name)
  {
    return name.Trim().ToLowerInvariant();

  }

  [GeneratedRegex(@"\s+")]
  private static partial Regex CanonicalRegex();

  private static string GenerateCanonicalName(string name)
  {
    return CanonicalRegex().Replace(name.Trim(), " ");
  }
}
