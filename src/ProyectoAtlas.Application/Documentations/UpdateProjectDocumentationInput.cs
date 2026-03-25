using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Documentations;

public record UpdateProjectDocumentationInput(
    string? Title,
    string? ContentMarkdown,
    int? SortOrder,
    DocumentationKind? Kind,
    DocumentationStatus? Status);
