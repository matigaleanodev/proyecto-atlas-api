using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public sealed class DuplicateDocumentationSlugException(string slug)
    : KnownException(
        $"Documentation slug '{slug}' already exists for the current project.",
        AtlasErrorCodes.DocumentationSlugConflict,
        HttpStatusCode.Conflict);
