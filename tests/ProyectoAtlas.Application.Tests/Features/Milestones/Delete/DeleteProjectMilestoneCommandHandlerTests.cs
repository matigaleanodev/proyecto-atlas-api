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
    DeleteProjectMilestoneCommandHandler handler = new(
        milestoneRepository,
        new FakeMilestoneFeatureLinkRepository(),
        projectRepository);

    await handler.Execute("proyecto-atlas", milestone.Slug);

    Assert.Same(milestone, milestoneRepository.DeletedMilestone);
  }

  [Fact]
  public async Task Execute_ShouldThrowMilestoneNotFoundException_WhenSlugDoesNotExist()
  {
    Project project = CreateProject();
    DeleteProjectMilestoneCommandHandler handler = new(
        new FakeMilestoneRepository(),
        new FakeMilestoneFeatureLinkRepository(),
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<MilestoneNotFoundException>(() => handler.Execute("proyecto-atlas", "missing-milestone"));
  }

  [Fact]
  public async Task Execute_ShouldThrowMilestoneDeleteBlockedException_WhenFeatureLinksExist()
  {
    Project project = CreateProject();
    Milestone milestone = new(project.Id, "MVP Release", "Cerrar la primera entrega publica.", MilestoneStatus.Planned);
    DeleteProjectMilestoneCommandHandler handler = new(
        new FakeMilestoneRepository { MilestoneBySlug = milestone },
        new FakeMilestoneFeatureLinkRepository
        {
          MilestoneLinks =
          [
            new MilestoneFeatureLink(project.Id, milestone.Id, Guid.NewGuid())
          ]
        },
        new FakeProjectRepository { ProjectBySlug = project });

    await Assert.ThrowsAsync<MilestoneDeleteBlockedException>(() => handler.Execute("proyecto-atlas", milestone.Slug));
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
