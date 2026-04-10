using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Common;

public sealed class InvalidProjectRelationItemException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.ProjectRelationInvalid,
        HttpStatusCode.BadRequest);
