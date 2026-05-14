using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Documentations.Common;

public sealed class DocumentationDeleteBlockedException(string projectSlug, string slug, string reason)
    : KnownException(
        $"Documentation with slug '{slug}' cannot be deleted for project '{projectSlug}' because {reason}.",
        AtlasErrorCodes.DocumentationDeleteBlocked,
        HttpStatusCode.Conflict);
