using ProyectoAtlas.Application.Features.DocumentationVersions.Common;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.DocumentationVersions.GetByVersionNumber;

public class GetDocumentationVersionByNumberQueryHandler(
    IDocumentationVersionRepository documentationVersionRepository,
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<DocumentationVersion> Execute(
      string projectSlug,
      string documentationSlug,
      int versionNumber,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(documentationSlug);

    if (versionNumber < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(versionNumber), "Version number must be greater than 0.");
    }

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, documentationSlug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, documentationSlug);

    return await documentationVersionRepository.GetByVersionNumber(
        documentation.Id,
        versionNumber,
        cancellationToken)
        ?? throw new DocumentationVersionNotFoundException(projectSlug, documentationSlug, versionNumber);
  }
}
