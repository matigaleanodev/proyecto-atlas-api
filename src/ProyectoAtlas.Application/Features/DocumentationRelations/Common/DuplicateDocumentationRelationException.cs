using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Common;

public sealed class DuplicateDocumentationRelationException()
    : KnownException(
        "Documentation relation already exists for the current project.",
        AtlasErrorCodes.DocumentationRelationConflict,
        HttpStatusCode.Conflict);
