using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Application.Features.ProjectRelations.List;

namespace ProyectoAtlas.Application.Features.ProjectRelations.ListIncoming;

public class ListIncomingProjectRelationsQueryHandler(
    IProjectRelationRepository projectRelationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ListProjectRelationsResponse> Execute(
      string targetProjectSlug,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(targetProjectSlug);

    Project targetProject = await projectRepository.GetBySlug(targetProjectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(targetProjectSlug);

    IReadOnlyCollection<ProjectRelation> relations = await projectRelationRepository.GetIncomingList(
        targetProject.Id,
        cancellationToken);

    return new ListProjectRelationsResponse(relations);
  }
}
