using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationVersions.Common;

public sealed class DocumentationVersionNotFoundException(string projectSlug, string documentationSlug, int versionNumber)
    : KnownException(
        $"Documentation version '{versionNumber}' was not found for documentation '{documentationSlug}' in project '{projectSlug}'.",
        AtlasErrorCodes.DocumentationVersionNotFound,
        HttpStatusCode.NotFound);
