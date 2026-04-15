using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Create;

public record CreateProjectRelationCommand(
    string TargetProjectSlug,
    ProjectRelationKind Kind);
