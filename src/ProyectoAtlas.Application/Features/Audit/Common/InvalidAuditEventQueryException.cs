using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Audit.Common;

public sealed class InvalidAuditEventQueryException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.ValidationError,
        HttpStatusCode.BadRequest);
