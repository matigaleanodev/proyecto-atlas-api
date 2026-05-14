using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.Common;

public sealed class FeatureDocumentationLinkNotFoundException(Guid linkId)
    : KnownException(
        $"Feature documentation link '{linkId}' was not found.",
        AtlasErrorCodes.FeatureDocumentationLinkNotFound,
        HttpStatusCode.NotFound);
