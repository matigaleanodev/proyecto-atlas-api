using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.DocumentationRelations.Create;

public class CreateDocumentationRelationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnRelation()
  {
    Project project = CreateProject();
    Documentation sourceDocumentation = CreateDocumentation(project.Id, "Getting Started");
    Documentation targetDocumentation = CreateDocumentation(project.Id, "ADR-001 Architecture", DocumentationKind.Decision, DocumentationArea.Architecture);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeDocumentationRelationRepository relationRepository = new();
    CreateDocumentationRelationCommandHandler handler = new(
        relationRepository,
        new QueueDocumentationRepository(new Queue<Documentation?>([sourceDocumentation, targetDocumentation])),
        projectRepository);
    CreateDocumentationRelationCommand input = new(targetDocumentation.Slug, DocumentationRelationKind.Implements);

    DocumentationRelation result = await handler.Execute("proyecto-atlas", sourceDocumentation.Slug, input);

    Assert.Equal(project.Id, result.ProjectId);
    Assert.Equal(sourceDocumentation.Id, result.SourceDocumentationId);
    Assert.Equal(sourceDocumentation.Id, relationRepository.AddedRelation!.SourceDocumentationId);
    Assert.Equal(input.Kind, result.Kind);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationRelationItemException_WhenRelationIsSelfReferential()
  {
    Project project = CreateProject();
    Documentation documentation = CreateDocumentation(project.Id, "Getting Started");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    Queue<Documentation?> queue = new([documentation, documentation]);
    FakeDocumentationRepository documentationRepository = new()
    {
      DocumentationBySlug = null
    };
    CreateDocumentationRelationCommandHandler handler = new(new FakeDocumentationRelationRepository(), new QueueDocumentationRepository(queue), projectRepository);

    await Assert.ThrowsAsync<InvalidDocumentationRelationItemException>(() =>
        handler.Execute("proyecto-atlas", documentation.Slug, new CreateDocumentationRelationCommand(documentation.Slug, DocumentationRelationKind.RelatedTo)));
  }

  [Fact]
  public async Task Execute_ShouldThrowDocumentationNotFoundException_WhenTargetDocumentationDoesNotExist()
  {
    Project project = CreateProject();
    Documentation sourceDocumentation = CreateDocumentation(project.Id, "Getting Started");
    CreateDocumentationRelationCommandHandler handler = new(
        new FakeDocumentationRelationRepository(),
        new QueueDocumentationRepository(new Queue<Documentation?>([sourceDocumentation, null])),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<DocumentationNotFoundException>(() =>
        handler.Execute("proyecto-atlas", sourceDocumentation.Slug, new CreateDocumentationRelationCommand("missing-doc", DocumentationRelationKind.RelatedTo)));
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }

  private static Documentation CreateDocumentation(
      Guid projectId,
      string title,
      DocumentationKind kind = DocumentationKind.Page,
      DocumentationArea area = DocumentationArea.Backend)
  {
    return new Documentation(
        projectId,
        title,
        "# Atlas",
        1,
        kind,
        DocumentationStatus.Draft,
        area);
  }

  private sealed class QueueDocumentationRepository(Queue<Documentation?> items) : IDocumentationRepository
  {
    public Task Add(Documentation documentation, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task Delete(Documentation documentation, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<Documentation?> GetBySlug(Guid projectId, string slug, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(items.Dequeue());
    }

    public Task<(IEnumerable<Documentation> Documentations, int TotalCount)> GetPagedList(
        Guid projectId,
        int page,
        int pageSize,
        string? query = null,
        DocumentationKind? kind = null,
        DocumentationStatus? status = null,
        DocumentationArea? area = null,
        string? tag = null,
        CancellationToken cancellationToken = default)
    {
      throw new NotSupportedException();
    }

    public Task Update(Documentation documentation, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task Update(
        Documentation documentation,
        DocumentationVersion? version,
        CancellationToken cancellationToken = default) => Task.CompletedTask;
  }
}
