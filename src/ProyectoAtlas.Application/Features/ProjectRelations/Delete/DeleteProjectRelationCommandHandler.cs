using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Delete;

public class DeleteProjectRelationCommandHandler(
    IProjectRelationRepository projectRelationRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(
      string sourceProjectSlug,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(sourceProjectSlug);

    Project sourceProject = await projectRepository.GetBySlug(sourceProjectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(sourceProjectSlug);

    ProjectRelation relation = await projectRelationRepository.GetById(
        sourceProject.Id,
        relationId,
        cancellationToken)
        ?? throw new ProjectRelationNotFoundException(relationId);

    await projectRelationRepository.Delete(relation, cancellationToken);
  }
}
