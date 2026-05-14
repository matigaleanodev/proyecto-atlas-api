using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Features.Common;

public sealed class FeatureDeleteBlockedException(string projectSlug, string slug, string reason)
    : KnownException(
        $"Feature with slug '{slug}' cannot be deleted for project '{projectSlug}' because {reason}.",
        AtlasErrorCodes.FeatureDeleteBlocked,
        HttpStatusCode.Conflict);
