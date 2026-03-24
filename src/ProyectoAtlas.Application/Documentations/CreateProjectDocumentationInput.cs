namespace ProyectoAtlas.Application.Documentations;

public record CreateProjectDocumentationInput(
    string Title,
    string ContentMarkdown,
    int SortOrder);
