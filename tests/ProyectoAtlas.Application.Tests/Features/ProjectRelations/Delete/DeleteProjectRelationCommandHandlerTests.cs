using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.ProjectRelations.Delete;

public class DeleteProjectRelationCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteRelation_WhenRelationExists()
  {
    Project project = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    ProjectRelation relation = new(project.Id, Guid.NewGuid(), ProjectRelationKind.DependsOn);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeProjectRelationRepository relationRepository = new()
    {
      RelationById = relation
    };
    DeleteProjectRelationCommandHandler handler = new(relationRepository, projectRepository);

    await handler.Execute(project.Slug, relation.Id);

    Assert.Same(relation, relationRepository.DeletedRelation);
    Assert.Equal(relation.Id, relationRepository.ReceivedRelationId);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectRelationNotFoundException_WhenRelationDoesNotExist()
  {
    Project project = CreateProject("Proyecto Atlas", "https://github.com/matigaleanodev/proyecto-atlas-api");
    DeleteProjectRelationCommandHandler handler = new(
        new FakeProjectRelationRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<ProjectRelationNotFoundException>(() => handler.Execute(project.Slug, Guid.NewGuid()));
  }

  private static Project CreateProject(string title, string repositoryUrl)
  {
    return new Project(
        title,
        "Backend for project documentation based on markdown",
        repositoryUrl,
        "#1E293B");
  }
}
