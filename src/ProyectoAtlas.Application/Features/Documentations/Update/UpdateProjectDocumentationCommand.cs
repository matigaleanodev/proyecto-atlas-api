using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.Documentations.Update;

public record UpdateProjectDocumentationCommand(
    string? Title,
    string? ContentMarkdown,
    int? SortOrder,
    DocumentationStatus? Status);
