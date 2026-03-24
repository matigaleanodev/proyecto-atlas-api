using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Projects;

public sealed class DuplicateProjectSlugException(string slug)
    : KnownException(
        $"Project slug '{slug}' already exists.",
        AtlasErrorCodes.ProjectSlugConflict,
        HttpStatusCode.Conflict);
