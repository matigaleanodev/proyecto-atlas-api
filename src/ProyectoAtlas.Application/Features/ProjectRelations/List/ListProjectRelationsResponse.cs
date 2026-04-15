using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectRelations.List;

public record ListProjectRelationsResponse(IReadOnlyCollection<ProjectRelation> Items);
