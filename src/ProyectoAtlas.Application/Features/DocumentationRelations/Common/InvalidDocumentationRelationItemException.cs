using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Common;

public sealed class InvalidDocumentationRelationItemException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.DocumentationRelationInvalid,
        HttpStatusCode.BadRequest);
