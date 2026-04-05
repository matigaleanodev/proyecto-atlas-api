using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Common;

public sealed class DuplicateDocumentationResourceException(string title)
    : KnownException(
        $"Documentation resource '{title}' already exists for this documentation.",
        AtlasErrorCodes.DocumentationResourceConflict,
        HttpStatusCode.Conflict);
