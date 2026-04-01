using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Features.Features.List;

public record ListProjectFeaturesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Query = null,
    FeatureStatus? Status = null);
