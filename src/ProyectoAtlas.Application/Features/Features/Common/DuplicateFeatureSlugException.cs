using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Features.Common;

public sealed class DuplicateFeatureSlugException(string slug)
    : KnownException(
        $"Feature slug '{slug}' already exists for the current project.",
        AtlasErrorCodes.FeatureSlugConflict,
        HttpStatusCode.Conflict);
