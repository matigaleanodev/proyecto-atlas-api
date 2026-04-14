using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ProjectRelations.Create;

public class CreateProjectRelationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnRelation()
  {
    Project sourceProject = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    Project targetProject = CreateProject("Atlas Docs", "https://github.com/matigaleanodev/atlas-docs");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = sourceProject
    };
    Queue<Project?> queuedProjects = new([sourceProject, targetProject]);
    FakeProjectRelationRepository relationRepository = new();
    CreateProjectRelationCommandHandler handler = new(relationRepository, new QueueProjectRepository(queuedProjects));
    CreateProjectRelationCommand command = new(targetProject.Slug, ProjectRelationKind.IntegratesWith);

    ProjectRelation result = await handler.Execute(sourceProject.Slug, command);

    Assert.Equal(sourceProject.Id, result.SourceProjectId);
    Assert.Equal(targetProject.Id, result.TargetProjectId);
    Assert.Equal(command.Kind, result.Kind);
    Assert.Same(result, relationRepository.AddedRelation);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidProjectRelationItemException_WhenRelationIsSelfReferential()
  {
    Project project = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    Queue<Project?> queuedProjects = new([project, project]);
    CreateProjectRelationCommandHandler handler = new(new FakeProjectRelationRepository(), new QueueProjectRepository(queuedProjects));
    CreateProjectRelationCommand command = new(project.Slug, ProjectRelationKind.DependsOn);

    await Assert.ThrowsAsync<InvalidProjectRelationItemException>(() => handler.Execute(project.Slug, command));
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenTargetProjectDoesNotExist()
  {
    Project sourceProject = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    Queue<Project?> queuedProjects = new([sourceProject, null]);
    CreateProjectRelationCommandHandler handler = new(new FakeProjectRelationRepository(), new QueueProjectRepository(queuedProjects));
    CreateProjectRelationCommand command = new("missing-project", ProjectRelationKind.Replaces);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => handler.Execute(sourceProject.Slug, command));
  }

  private static Project CreateProject(string title, string repositoryUrl)
  {
    return new Project(
        title,
        "Backend for project documentation based on markdown",
        repositoryUrl,
        "#1E293B");
  }

  private sealed class QueueProjectRepository(Queue<Project?> items) : IProjectRepository
  {
    public Task Add(Project project, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task Delete(Project project, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<Project?> GetBySlug(string slug, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(items.Dequeue());
    }

    public Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedList(
        int page,
        int pageSize,
        string? query = null,
        CancellationToken cancellationToken = default)
    {
      throw new NotSupportedException();
    }

    public Task Update(Project project, CancellationToken cancellationToken = default) => Task.CompletedTask;
  }
}
