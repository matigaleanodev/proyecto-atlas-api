using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public sealed class InvalidDocumentationTagsException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.DocumentationTagsInvalid,
        HttpStatusCode.BadRequest);
