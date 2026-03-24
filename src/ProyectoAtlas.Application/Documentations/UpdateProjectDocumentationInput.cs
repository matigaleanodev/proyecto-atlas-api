namespace ProyectoAtlas.Application.Documentations;

public record UpdateProjectDocumentationInput(
    string? Title,
    string? ContentMarkdown,
    int? SortOrder);
