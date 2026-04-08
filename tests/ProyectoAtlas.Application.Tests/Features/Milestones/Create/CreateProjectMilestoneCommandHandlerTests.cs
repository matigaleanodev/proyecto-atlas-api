using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Milestones.Create;

public class CreateProjectMilestoneCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnMilestone()
  {
    Project project = CreateProject();
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeMilestoneRepository milestoneRepository = new();
    CreateProjectMilestoneCommandHandler handler = new(milestoneRepository, projectRepository);
    DateTime targetDateUtc = new(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc);
    CreateProjectMilestoneCommand input = new("MVP Release", "Cerrar la primera entrega publica.", MilestoneStatus.Planned, targetDateUtc);

    Milestone result = await handler.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Summary, result.Summary);
    Assert.Equal(input.Status, result.Status);
    Assert.Equal(targetDateUtc, result.TargetDateUtc);
    Assert.Equal(project.Id, result.ProjectId);
    Assert.Same(result, milestoneRepository.AddedMilestone);
  }

  [Fact]
  public async Task Execute_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
  {
    Project project = CreateProject();
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    CreateProjectMilestoneCommandHandler handler = new(new FakeMilestoneRepository(), projectRepository);
    CreateProjectMilestoneCommand input = new("Lanzamiento MVP / Inicial", "Cerrar la primera entrega publica.", MilestoneStatus.Planned);

    Milestone result = await handler.Execute("proyecto-atlas", input);

    Assert.Equal("lanzamiento-mvp-inicial", result.Slug);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    CreateProjectMilestoneCommandHandler handler = new(new FakeMilestoneRepository(), new FakeProjectRepository());
    CreateProjectMilestoneCommand input = new("MVP Release", "Cerrar la primera entrega publica.", MilestoneStatus.Planned);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => handler.Execute("missing-project", input));
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
