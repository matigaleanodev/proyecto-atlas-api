using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectRelations.List;

public class ListProjectRelationsQueryHandler(
    IProjectRelationRepository projectRelationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListProjectRelationsResponse> Execute(
      string sourceProjectSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(sourceProjectSlug);

    Project sourceProject = await projectRepository.GetBySlug(sourceProjectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(sourceProjectSlug);

    IReadOnlyCollection<ProjectRelation> relations = await projectRelationRepository.GetOutgoingList(
        sourceProject.Id,
        cancellationToken);

    return new ListProjectRelationsResponse(relations);
  }
}
