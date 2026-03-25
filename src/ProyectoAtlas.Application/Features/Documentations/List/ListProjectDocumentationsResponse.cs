using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Documentations;

public record ListProjectDocumentationsResponse(
    IReadOnlyCollection<Documentation> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
