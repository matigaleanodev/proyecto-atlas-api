using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Common;

public sealed class DocumentationResourceNotFoundException(Guid resourceId)
    : KnownException(
        $"Documentation resource '{resourceId}' was not found.",
        AtlasErrorCodes.DocumentationResourceNotFound,
        HttpStatusCode.NotFound);
