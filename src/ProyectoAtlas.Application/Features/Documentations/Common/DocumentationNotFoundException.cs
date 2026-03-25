using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public sealed class DocumentationNotFoundException(string projectSlug, string slug)
    : KnownException(
        $"Documentation with slug '{slug}' was not found for project '{projectSlug}'.",
        AtlasErrorCodes.DocumentationNotFound,
        HttpStatusCode.NotFound);
