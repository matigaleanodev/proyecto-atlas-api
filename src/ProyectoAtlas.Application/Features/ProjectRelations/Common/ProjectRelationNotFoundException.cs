using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Common;

public sealed class ProjectRelationNotFoundException(Guid relationId)
    : KnownException(
        $"Project relation '{relationId}' was not found.",
        AtlasErrorCodes.ProjectRelationNotFound,
        HttpStatusCode.NotFound);
