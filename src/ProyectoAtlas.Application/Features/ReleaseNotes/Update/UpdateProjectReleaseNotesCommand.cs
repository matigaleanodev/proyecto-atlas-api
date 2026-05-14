using ProyectoAtlas.Application.Features.Documentations.Update;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.Update;

public record UpdateProjectReleaseNotesCommand(
    string? Title,
    string? ContentMarkdown,
    int? SortOrder,
    DocumentationStatus? Status,
    IReadOnlyCollection<UpdateProjectDocumentationTag>? Tags = null);
