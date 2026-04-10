using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Common;

public sealed class DuplicateProjectRelationException()
    : KnownException(
        "Project relation already exists.",
        AtlasErrorCodes.ProjectRelationConflict,
        HttpStatusCode.Conflict);
