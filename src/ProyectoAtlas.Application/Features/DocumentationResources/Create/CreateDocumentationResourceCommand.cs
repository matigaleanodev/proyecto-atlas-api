using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationResources.Create;

public record CreateDocumentationResourceCommand(
    string Title,
    string Url,
    DocumentationResourceKind Kind,
    string? Description = null,
    int SortOrder = 1);
