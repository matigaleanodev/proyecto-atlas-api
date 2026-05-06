using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Projects.Common;

public sealed class ProjectDeleteBlockedException(string slug, string reason)
    : KnownException(
        $"Project with slug '{slug}' cannot be deleted because {reason}.",
        AtlasErrorCodes.ProjectDeleteBlocked,
        HttpStatusCode.Conflict);
