using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public sealed class InvalidDocumentationTitleConventionException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.DocumentationTitleConventionInvalid,
        HttpStatusCode.BadRequest);
