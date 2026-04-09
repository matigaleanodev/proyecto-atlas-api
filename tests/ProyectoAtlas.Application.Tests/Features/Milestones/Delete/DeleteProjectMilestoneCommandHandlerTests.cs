using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Milestones.Delete;

public class DeleteProjectMilestoneCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteMilestone_WhenSlugExists()
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
    DeleteProjectMilestoneCommandHandler handler = new(milestoneRepository, projectRepository);

    await handler.Execute("proyecto-atlas", milestone.Slug);

    Assert.Same(milestone, milestoneRepository.DeletedMilestone);
  }

  [Fact]
  public async Task Execute_ShouldThrowMilestoneNotFoundException_WhenSlugDoesNotExist()
  {
    Project project = CreateProject();
    DeleteProjectMilestoneCommandHandler handler = new(
        new FakeMilestoneRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<MilestoneNotFoundException>(() => handler.Execute("proyecto-atlas", "missing-milestone"));
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
