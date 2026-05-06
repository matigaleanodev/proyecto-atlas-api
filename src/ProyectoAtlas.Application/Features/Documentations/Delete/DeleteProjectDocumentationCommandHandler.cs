using ProyectoAtlas.Application.Features.DocumentationRelations.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.Delete;

public class DeleteProjectDocumentationCommandHandler(
    IDocumentationRepository documentationRepository,
    IDocumentationRelationRepository documentationRelationRepository,
    IFeatureDocumentationLinkRepository featureDocumentationLinkRepository,
    IAuditEventRepository auditEventRepository,
    IProjectRepository projectRepository)
{
  public async Task Execute(string projectSlug, string slug, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, slug);

    IReadOnlyCollection<DocumentationRelation> outgoingRelations =
        await documentationRelationRepository.GetOutgoingList(documentation.Id, cancellationToken);

    if (outgoingRelations.Count > 0)
    {
      throw new DocumentationDeleteBlockedException(projectSlug, slug, "it still has outgoing documentation relations");
    }

    IReadOnlyCollection<DocumentationRelation> incomingRelations =
        await documentationRelationRepository.GetIncomingList(documentation.Id, cancellationToken);

    if (incomingRelations.Count > 0)
    {
      throw new DocumentationDeleteBlockedException(projectSlug, slug, "it is still referenced by other documentation items");
    }

    IReadOnlyCollection<Domain.Features.FeatureDocumentationLink> linkedFeatures =
        await featureDocumentationLinkRepository.GetByDocumentationId(documentation.Id, cancellationToken);

    if (linkedFeatures.Count > 0)
    {
      throw new DocumentationDeleteBlockedException(projectSlug, slug, "it is still linked to one or more features");
    }

    await auditEventRepository.Add(AuditEventFactory.ForDocumentation(documentation, Domain.Audit.AuditAction.Deleted), cancellationToken);
    await documentationRepository.Delete(documentation, cancellationToken);
  }
}
