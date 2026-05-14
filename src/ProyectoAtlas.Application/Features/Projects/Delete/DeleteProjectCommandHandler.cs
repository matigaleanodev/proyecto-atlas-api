using ProyectoAtlas.Application.Features.Features.Common;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Delete;

public class DeleteProjectCommandHandler(
    IDocumentationRepository documentationRepository,
    IFeatureRepository featureRepository,
    IMilestoneRepository milestoneRepository,
    IProjectRelationRepository projectRelationRepository,
    IProjectRepository projectRepository,
    IAuditEventRepository auditEventRepository)
{
  public async Task Execute(string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(slug, cancellationToken)
        ?? throw new ProjectNotFoundException(slug);

    (IEnumerable<Domain.Documentations.Documentation> _, int documentationCount) =
        await documentationRepository.GetPagedList(project.Id, 1, 1, cancellationToken: cancellationToken);

    if (documentationCount > 0)
    {
      throw new ProjectDeleteBlockedException(slug, "it still has documentations");
    }

    (IEnumerable<Domain.Features.Feature> _, int featureCount) =
        await featureRepository.GetPagedList(project.Id, 1, 1, cancellationToken: cancellationToken);

    if (featureCount > 0)
    {
      throw new ProjectDeleteBlockedException(slug, "it still has features");
    }

    (IEnumerable<Domain.Milestones.Milestone> _, int milestoneCount) =
        await milestoneRepository.GetPagedList(project.Id, 1, 1, cancellationToken: cancellationToken);

    if (milestoneCount > 0)
    {
      throw new ProjectDeleteBlockedException(slug, "it still has milestones");
    }

    IReadOnlyCollection<ProjectRelation> outgoingRelations =
        await projectRelationRepository.GetOutgoingList(project.Id, cancellationToken);

    if (outgoingRelations.Count > 0)
    {
      throw new ProjectDeleteBlockedException(slug, "it still has outgoing project relations");
    }

    IReadOnlyCollection<ProjectRelation> incomingRelations =
        await projectRelationRepository.GetIncomingList(project.Id, cancellationToken);

    if (incomingRelations.Count > 0)
    {
      throw new ProjectDeleteBlockedException(slug, "it is still referenced by other projects");
    }

    await auditEventRepository.Add(AuditEventFactory.ForProject(project, Domain.Audit.AuditAction.Deleted), cancellationToken);
    await projectRepository.Delete(project, cancellationToken);
  }
}
