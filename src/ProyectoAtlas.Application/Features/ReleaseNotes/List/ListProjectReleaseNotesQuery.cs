using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.ReleaseNotes.List;

public record ListProjectReleaseNotesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Query = null,
    DocumentationStatus? Status = null,
    DocumentationArea? Area = null,
    string? Tag = null);
