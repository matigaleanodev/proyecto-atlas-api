using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.FeatureDocumentationLinks.Common;

public sealed class DuplicateFeatureDocumentationLinkException()
    : KnownException(
        "Feature documentation link already exists for the current project.",
        AtlasErrorCodes.FeatureDocumentationLinkConflict,
        HttpStatusCode.Conflict);
