using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.ProjectRelations.Create;

public class CreateProjectRelationCommandHandler(
    IProjectRelationRepository projectRelationRepository,
    IProjectRepository projectRepository)
{
  public async Task<ProjectRelation> Execute(
      string sourceProjectSlug,
      CreateProjectRelationCommand command,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(sourceProjectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(command.TargetProjectSlug);

    Project sourceProject = await projectRepository.GetBySlug(sourceProjectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(sourceProjectSlug);

    Project targetProject = await projectRepository.GetBySlug(command.TargetProjectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(command.TargetProjectSlug);

    ProjectRelation relation;

    try
    {
      relation = new ProjectRelation(sourceProject.Id, targetProject.Id, command.Kind);
    }
    catch (InvalidProjectRelationException exception)
    {
      throw new InvalidProjectRelationItemException(exception.Message);
    }

    await projectRelationRepository.Add(relation, cancellationToken);

    return relation;
  }
}
