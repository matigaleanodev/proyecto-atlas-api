using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Projects;

public sealed class ProjectNotFoundException(string slug)
    : KnownException(
        $"Project with slug '{slug}' was not found.",
        AtlasErrorCodes.ProjectNotFound,
        HttpStatusCode.NotFound);
