using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Documentations;

public record UpdateProjectDocumentationCommand(
    string? Title,
    string? ContentMarkdown,
    int? SortOrder,
    DocumentationStatus? Status);
