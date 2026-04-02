using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.List;

public record ListDocumentationRelationsResponse(IReadOnlyCollection<DocumentationRelation> Items);
