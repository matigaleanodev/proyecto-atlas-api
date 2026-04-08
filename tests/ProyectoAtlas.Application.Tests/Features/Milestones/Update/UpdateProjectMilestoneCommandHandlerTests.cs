using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Milestones.Update;

public class UpdateProjectMilestoneCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldUpdateMilestone_WhenSlugExists()
  {
    Project project = CreateProject();
    Milestone milestone = new(project.Id, "MVP Release", "Cerrar la primera entrega publica.", MilestoneStatus.Planned);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeMilestoneRepository milestoneRepository = new()
    {
      MilestoneBySlug = milestone
    };
    UpdateProjectMilestoneCommandHandler handler = new(milestoneRepository, projectRepository);
    DateTime targetDateUtc = new(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc);
    UpdateProjectMilestoneCommand input = new("GA Release", "Cerrar la entrega general.", MilestoneStatus.InProgress, targetDateUtc);

    Milestone result = await handler.Execute("proyecto-atlas", milestone.Slug, input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Summary, result.Summary);
    Assert.Equal(input.Status, result.Status);
    Assert.Equal(targetDateUtc, result.TargetDateUtc);
    Assert.Same(result, milestoneRepository.UpdatedMilestone);
  }

  [Fact]
  public async Task Execute_ShouldClearTargetDate_WhenRequested()
  {
    Project project = CreateProject();
    Milestone milestone = new(
        project.Id,
        "MVP Release",
        "Cerrar la primera entrega publica.",
        MilestoneStatus.Planned,
        new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc));
    UpdateProjectMilestoneCommandHandler handler = new(
        new FakeMilestoneRepository { MilestoneBySlug = milestone },
        new FakeProjectRepository { ProjectBySlug = project });
    UpdateProjectMilestoneCommand input = new(null, null, null, null, true);

    Milestone result = await handler.Execute("proyecto-atlas", milestone.Slug, input);

    Assert.Null(result.TargetDateUtc);
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
