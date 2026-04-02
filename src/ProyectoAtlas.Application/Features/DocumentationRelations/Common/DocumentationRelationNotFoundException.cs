using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Common;

public sealed class DocumentationRelationNotFoundException(string projectSlug, string documentationSlug, Guid relationId)
    : KnownException(
        $"Documentation relation '{relationId}' was not found for documentation '{documentationSlug}' in project '{projectSlug}'.",
        AtlasErrorCodes.DocumentationRelationNotFound,
        HttpStatusCode.NotFound);
