using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public sealed class InvalidDocumentationFaqItemsException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.DocumentationFaqItemsInvalid,
        HttpStatusCode.BadRequest);
