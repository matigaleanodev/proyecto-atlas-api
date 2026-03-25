using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Documentations;

public record CreateProjectDocumentationCommand(
    string Title,
    string ContentMarkdown,
    int SortOrder,
    DocumentationKind Kind,
    DocumentationStatus Status);
