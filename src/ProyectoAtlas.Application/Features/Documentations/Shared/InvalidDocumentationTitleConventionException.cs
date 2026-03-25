using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Documentations;

public sealed class InvalidDocumentationTitleConventionException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.DocumentationTitleConventionInvalid,
        HttpStatusCode.BadRequest);
