using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Application.Features.Features.Update;

public record UpdateProjectFeatureCommand(
    string? Title,
    string? Summary,
    FeatureStatus? Status);
