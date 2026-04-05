using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationResources.List;

public record ListDocumentationResourcesResponse(IReadOnlyCollection<DocumentationResource> Items);
