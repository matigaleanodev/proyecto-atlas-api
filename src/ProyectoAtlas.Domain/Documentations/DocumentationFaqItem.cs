namespace ProyectoAtlas.Domain.Documentations;

public class DocumentationFaqItem
{
  private DocumentationFaqItem()
  {
  }

  public DocumentationFaqItem(Guid documentationId, string question, string answer, int sortOrder)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(question);
    ArgumentException.ThrowIfNullOrWhiteSpace(answer);

    if (documentationId == Guid.Empty)
    {
      throw new ArgumentException("Documentation id is required.", nameof(documentationId));
    }

    if (sortOrder < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(sortOrder), "Sort order must be greater than 0.");
    }

    Id = Guid.NewGuid();
    DocumentationId = documentationId;
    Question = question.Trim();
    Answer = answer.Trim();
    SortOrder = sortOrder;
  }

  public Guid Id { get; private set; }
  public Guid DocumentationId { get; private set; }
  public string Question { get; private set; } = null!;
  public string Answer { get; private set; } = null!;
  public int SortOrder { get; private set; }

  public void Update(string question, string answer, int sortOrder)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(question);
    ArgumentException.ThrowIfNullOrWhiteSpace(answer);

    if (sortOrder < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(sortOrder), "Sort order must be greater than 0.");
    }

    Question = question.Trim();
    Answer = answer.Trim();
    SortOrder = sortOrder;
  }
}
