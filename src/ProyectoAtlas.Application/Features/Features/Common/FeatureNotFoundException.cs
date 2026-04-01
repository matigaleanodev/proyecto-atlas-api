using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Features.Common;

public sealed class FeatureNotFoundException(string projectSlug, string featureSlug)
    : KnownException(
        $"Feature with slug '{featureSlug}' was not found for project '{projectSlug}'.",
        AtlasErrorCodes.FeatureNotFound,
        HttpStatusCode.NotFound);
