using ProyectoAtlas.Application.Features.Documentations.Create;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.Create;

public record CreateProjectReleaseNotesCommand(
    string Title,
    string ContentMarkdown,
    int SortOrder,
    DocumentationStatus Status,
    DocumentationArea Area,
    IReadOnlyCollection<CreateProjectDocumentationTag>? Tags = null);
