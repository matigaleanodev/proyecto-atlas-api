using ProyectoAtlas.Application.Features.DocumentationRelations.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationRelations.Create;

public class CreateDocumentationRelationCommandHandler(
    IDocumentationRelationRepository documentationRelationRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<DocumentationRelation> Execute(
      string projectSlug,
      string sourceDocumentationSlug,
      CreateDocumentationRelationCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(sourceDocumentationSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.TargetDocumentationSlug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation sourceDocumentation = await documentationRepository.GetBySlug(project.Id, sourceDocumentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, sourceDocumentationSlug);

    Documentation targetDocumentation = await documentationRepository.GetBySlug(project.Id, input.TargetDocumentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, input.TargetDocumentationSlug);

    try
    {
      DocumentationRelation relation = new(
          project.Id,
          sourceDocumentation.Id,
          targetDocumentation.Id,
          input.Kind);

      await documentationRelationRepository.Add(relation, cancellationToken);

      return relation;
    }
    catch (InvalidDocumentationRelationException exception)
    {
      throw new InvalidDocumentationRelationItemException(exception.Message);
    }
  }
}
