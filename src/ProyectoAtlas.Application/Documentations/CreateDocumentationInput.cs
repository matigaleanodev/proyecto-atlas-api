namespace ProyectoAtlas.Application.Documentations;

public record CreateDocumentationInput(
    string Title,
    string ContentMarkdown,
    int SortOrder);
