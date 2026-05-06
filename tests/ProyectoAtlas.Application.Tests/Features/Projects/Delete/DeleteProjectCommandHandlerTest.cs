using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Projects.Delete;

public class DeleteProjectCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteProject_WhenSlugExists()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    FakeAuditEventRepository auditEventRepository = new();
    DeleteProjectCommandHandler useCase = new DeleteProjectCommandHandler(
        new FakeDocumentationRepository(),
        new FakeFeatureRepository(),
        new FakeMilestoneRepository(),
        new FakeProjectRelationRepository(),
        projectRepository,
        auditEventRepository);

    await useCase.Execute("proyecto-atlas");

    Assert.Same(existingProject, projectRepository.DeletedProject);
    Assert.NotNull(auditEventRepository.AddedAuditEvent);
    Assert.Equal(Domain.Audit.AuditAction.Deleted, auditEventRepository.AddedAuditEvent.Action);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    DeleteProjectCommandHandler useCase = new DeleteProjectCommandHandler(
        new FakeDocumentationRepository(),
        new FakeFeatureRepository(),
        new FakeMilestoneRepository(),
        new FakeProjectRelationRepository(),
        new FakeProjectRepository(),
        new FakeAuditEventRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => useCase.Execute("missing-project"));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenSlugIsInvalid(string? slug)
  {
    DeleteProjectCommandHandler useCase = new DeleteProjectCommandHandler(
        new FakeDocumentationRepository(),
        new FakeFeatureRepository(),
        new FakeMilestoneRepository(),
        new FakeProjectRelationRepository(),
        new FakeProjectRepository(),
        new FakeAuditEventRepository());

    await Assert.ThrowsAnyAsync<ArgumentException>(() => useCase.Execute(slug!));
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectDeleteBlockedException_WhenDocumentationsExist()
  {
    Project project = CreateProject();
    DeleteProjectCommandHandler useCase = new(
        new FakeDocumentationRepository { PagedTotalCount = 1 },
        new FakeFeatureRepository(),
        new FakeMilestoneRepository(),
        new FakeProjectRelationRepository(),
        new FakeProjectRepository { ProjectBySlug = project },
        new FakeAuditEventRepository());

    await Assert.ThrowsAsync<ProjectDeleteBlockedException>(() => useCase.Execute(project.Slug));
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectDeleteBlockedException_WhenIncomingRelationsExist()
  {
    Project project = CreateProject();
    DeleteProjectCommandHandler useCase = new(
        new FakeDocumentationRepository(),
        new FakeFeatureRepository(),
        new FakeMilestoneRepository(),
        new FakeProjectRelationRepository
        {
          IncomingRelations =
          [
            new ProjectRelation(Guid.NewGuid(), project.Id, ProjectRelationKind.DependsOn)
          ]
        },
        new FakeProjectRepository { ProjectBySlug = project },
        new FakeAuditEventRepository());

    await Assert.ThrowsAsync<ProjectDeleteBlockedException>(() => useCase.Execute(project.Slug));
  }

  private static Project CreateProject()
  {
    return new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
  }
}
