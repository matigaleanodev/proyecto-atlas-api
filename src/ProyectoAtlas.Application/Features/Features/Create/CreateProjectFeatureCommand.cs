using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Features.Features.Create;

public record CreateProjectFeatureCommand(
    string Title,
    string Summary,
    FeatureStatus Status);
