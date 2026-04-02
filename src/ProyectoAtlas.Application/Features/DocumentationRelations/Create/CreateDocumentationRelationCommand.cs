using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Create;

public record CreateDocumentationRelationCommand(
    string TargetDocumentationSlug,
    DocumentationRelationKind Kind);
