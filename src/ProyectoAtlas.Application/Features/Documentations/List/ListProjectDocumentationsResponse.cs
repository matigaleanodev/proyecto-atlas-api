using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.Documentations.List;

public record ListProjectDocumentationsResponse(
    IReadOnlyCollection<Documentation> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
