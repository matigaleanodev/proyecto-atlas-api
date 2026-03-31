using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Projects.Common;

public sealed class InvalidProjectLinkItemException(string message)
    : KnownException(
        message,
        AtlasErrorCodes.ProjectLinkItemInvalid,
        HttpStatusCode.BadRequest);
