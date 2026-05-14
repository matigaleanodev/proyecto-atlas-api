using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationResources.List;

public record ListDocumentationResourcesQuery(DocumentationResourceKind? Kind = null);
