using ProyectoAtlas.Application.Features.DocumentationVersions.GetByVersionNumber;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationVersions.GetByVersionNumber;

public class GetDocumentationVersionByNumberQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnVersion()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    DocumentationVersion version = new(
        documentation.Id,
        2,
        "Quick Start",
        "## Updated",
        DocumentationStatus.Published);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeDocumentationVersionRepository versionRepository = new()
    {
      VersionByNumber = version
    };
    GetDocumentationVersionByNumberQueryHandler handler = new(versionRepository, documentationRepository, projectRepository);

    DocumentationVersion result = await handler.Execute("proyecto-atlas", documentation.Slug, 2);

    Assert.Same(version, result);
    Assert.Equal(documentation.Id, versionRepository.ReceivedDocumentationId);
    Assert.Equal(2, versionRepository.ReceivedVersionNumber);
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationVersionNotFoundException_WhenVersionDoesNotExist()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    GetDocumentationVersionByNumberQueryHandler handler = new(
        new FakeDocumentationVersionRepository(),
        new FakeDocumentationRepository { DocumentationBySlug = documentation },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationVersionNotFoundException>(() =>
        handler.Execute("proyecto-atlas", documentation.Slug, 1));
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Documentation CreateDocumentation(Guid projectId, string title)
  {
    return new Documentation(
        projectId,
        title,
        "# Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
  }
}
