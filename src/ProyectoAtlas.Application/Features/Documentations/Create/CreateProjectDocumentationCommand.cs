using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.Documentations.Create;

public record CreateProjectDocumentationCommand(
    string Title,
    string ContentMarkdown,
    int SortOrder,
    DocumentationKind Kind,
    DocumentationStatus Status,
    DocumentationArea Area);
