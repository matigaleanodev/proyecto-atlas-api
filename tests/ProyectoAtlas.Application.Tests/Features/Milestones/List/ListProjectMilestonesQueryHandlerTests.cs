using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Milestones.List;

public class ListProjectMilestonesQueryHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnPagedMilestones()
  {
    Project project = CreateProject();
    Milestone milestone = new(project.Id, "MVP Release", "Cerrar la primera entrega publica.", MilestoneStatus.Planned);
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = project
    };
    FakeMilestoneRepository milestoneRepository = new()
    {
      PagedMilestones = [milestone],
      PagedTotalCount = 1
    };
    ListProjectMilestonesQueryHandler handler = new(milestoneRepository, projectRepository);

    ListProjectMilestonesResponse result = await handler.Execute("proyecto-atlas", new ListProjectMilestonesQuery());

    Assert.Single(result.Items);
    Assert.Equal(1, result.TotalItems);
    Assert.Equal(project.Id, milestoneRepository.ReceivedProjectId);
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
