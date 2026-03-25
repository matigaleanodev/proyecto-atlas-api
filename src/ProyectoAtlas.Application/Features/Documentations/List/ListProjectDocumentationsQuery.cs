using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Documentations;

public record ListProjectDocumentationsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Query = null,
    DocumentationKind? Kind = null,
    DocumentationStatus? Status = null);
