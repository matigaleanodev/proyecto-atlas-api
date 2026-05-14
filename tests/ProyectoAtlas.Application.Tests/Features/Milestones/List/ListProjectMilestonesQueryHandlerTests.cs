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
      PagedTotalCount = 2
    };
    ListProjectMilestonesQueryHandler handler = new(milestoneRepository, projectRepository);
    ListProjectMilestonesQuery input = new(
        Page: 2,
        PageSize: 1,
        Query: "release",
        Status: MilestoneStatus.Planned,
        TargetDateUtcFrom: new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
        TargetDateUtcTo: new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc));

    ListProjectMilestonesResponse result = await handler.Execute("proyecto-atlas", input);

    Assert.Single(result.Items);
    Assert.Equal(2, result.TotalItems);
    Assert.Equal(2, result.TotalPages);
    Assert.Equal(project.Id, milestoneRepository.ReceivedProjectId);
    Assert.Equal(input.Page, milestoneRepository.ReceivedPage);
    Assert.Equal(input.PageSize, milestoneRepository.ReceivedPageSize);
    Assert.Equal(input.Query, milestoneRepository.ReceivedQuery);
    Assert.Equal(input.Status, milestoneRepository.ReceivedStatus);
    Assert.Equal(input.TargetDateUtcFrom, milestoneRepository.ReceivedTargetDateUtcFrom);
    Assert.Equal(input.TargetDateUtcTo, milestoneRepository.ReceivedTargetDateUtcTo);
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
