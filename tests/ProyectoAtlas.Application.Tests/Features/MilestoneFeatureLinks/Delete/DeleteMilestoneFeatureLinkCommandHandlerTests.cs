using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.MilestoneFeatureLinks.Delete;

public class DeleteMilestoneFeatureLinkCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldDeleteLink_WhenLinkExists()
  {
    Project project = CreateProject();
    Milestone milestone = CreateMilestone(project.Id, "MVP Release");
    MilestoneFeatureLink link = new(project.Id, milestone.Id, Guid.NewGuid());
    FakeMilestoneFeatureLinkRepository linkRepository = new()
    {
      LinkById = link
    };
    DeleteMilestoneFeatureLinkCommandHandler handler = new(
        linkRepository,
        new FakeMilestoneRepository { MilestoneBySlug = milestone },
        new FakeProjectRepository { ProjectBySlug = project });

    await handler.Execute(project.Slug, milestone.Slug, link.Id);

    Assert.Same(link, linkRepository.DeletedLink);
  }

  private static Project CreateProject()
  {
    return new Project("Proyecto Atlas", "Backend for project documentation based on markdown", "https://github.com/matigaleanodev/proyecto-atlas-api", "#1E293B");
  }

  private static Milestone CreateMilestone(Guid projectId, string title)
  {
    return new Milestone(projectId, title, "Cerrar la primera entrega publica.", MilestoneStatus.Planned);
  }
}
