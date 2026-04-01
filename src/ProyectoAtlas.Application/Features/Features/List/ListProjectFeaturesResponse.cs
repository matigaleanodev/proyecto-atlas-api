using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Features.Features.List;

public record ListProjectFeaturesResponse(
    IReadOnlyCollection<Feature> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
