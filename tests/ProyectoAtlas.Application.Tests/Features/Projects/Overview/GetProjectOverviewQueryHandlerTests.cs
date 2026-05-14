using ProyectoAtlas.Application.Features.Projects.Overview;
using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Projects.Overview;

public class GetProjectOverviewQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnProjectOverview()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    AuditEvent auditEvent = new(
        project.Id,
        documentationId: null,
        AuditEntityType.Project,
        project.Id,
        project.Slug,
        project.Title,
        AuditAction.Created);
    FakeProjectOverviewRepository overviewRepository = new()
    {
      Overview = new ProjectOverviewSummary(
          project.Id,
          project.Slug,
          project.Title,
          2,
          3,
          1,
          1,
          0,
          [auditEvent])
    };
    GetProjectOverviewQueryHandler handler = new(overviewRepository, projectRepository);

    ProjectOverviewSummary result = await handler.Execute(project.Slug);

    Assert.Equal(project.Id, overviewRepository.ReceivedProjectId);
    Assert.Equal(project.Slug, result.ProjectSlug);
    Assert.Equal(2, result.DocumentationCount);
    Assert.Single(result.RecentActivity);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    GetProjectOverviewQueryHandler handler = new(new FakeProjectOverviewRepository(), new FakeProjectRepository());

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => handler.Execute("missing-project"));
  }
}
