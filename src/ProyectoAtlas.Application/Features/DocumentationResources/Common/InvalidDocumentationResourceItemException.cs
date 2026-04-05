using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Common;

public sealed class InvalidDocumentationResourceItemException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.DocumentationResourceInvalid,
        HttpStatusCode.BadRequest);
