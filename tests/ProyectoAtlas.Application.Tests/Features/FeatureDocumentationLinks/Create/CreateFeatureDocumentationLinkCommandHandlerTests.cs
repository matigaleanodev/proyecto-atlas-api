using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.FeatureDocumentationLinks.Create;

public class CreateFeatureDocumentationLinkCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnLink()
  {
    Project project = CreateProject();
    Feature feature = CreateFeature(project.Id, "Authentication API");
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeFeatureRepository featureRepository = new()
    {
      FeatureBySlug = feature
    };
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = documentation
    };
    FakeFeatureDocumentationLinkRepository linkRepository = new();
    CreateFeatureDocumentationLinkCommandHandler handler = new(
        linkRepository,
        featureRepository,
        documentationRepository,
        projectRepository);
    CreateFeatureDocumentationLinkCommand command = new(documentation.Slug);

    FeatureDocumentationLink result = await handler.Execute(project.Slug, feature.Slug, command);

    Assert.Equal(project.Id, result.ProjectId);
    Assert.Equal(feature.Id, result.FeatureId);
    Assert.Equal(documentation.Id, result.DocumentationId);
    Assert.Same(result, linkRepository.AddedLink);
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationNotFoundException_WhenDocumentationDoesNotExist()
  {
    Project project = CreateProject();
    Feature feature = CreateFeature(project.Id, "Authentication API");
    CreateFeatureDocumentationLinkCommandHandler handler = new(
        new FakeFeatureDocumentationLinkRepository(),
        new FakeFeatureRepository { FeatureBySlug = feature },
        new FakeDocumentationRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() =>
        handler.Execute(project.Slug, feature.Slug, new CreateFeatureDocumentationLinkCommand("missing-doc")));
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Feature CreateFeature(Guid projectId, string title)
  {
    return new Feature(projectId, title, "Expose login endpoints.", FeatureStatus.Planned);
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
