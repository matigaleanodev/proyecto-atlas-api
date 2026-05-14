using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.MilestoneFeatureLinks.Create;

public class CreateMilestoneFeatureLinkCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldReturnLink()
  {
    Project project = CreateProject();
    Milestone milestone = CreateMilestone(project.Id, "MVP Release");
    Feature feature = CreateFeature(project.Id, "Authentication API");
    CreateMilestoneFeatureLinkCommandHandler handler = new(
        new FakeMilestoneFeatureLinkRepository(),
        new FakeFeatureRepository { FeatureBySlug = feature },
        new FakeMilestoneRepository { MilestoneBySlug = milestone },
        new FakeProjectRepository { ProjectBySlug = project });

    MilestoneFeatureLink result = await handler.Execute(project.Slug, milestone.Slug, new CreateMilestoneFeatureLinkCommand(feature.Slug));

    Assert.Equal(project.Id, result.ProjectId);
    Assert.Equal(milestone.Id, result.MilestoneId);
    Assert.Equal(feature.Id, result.FeatureId);
  }

  private static Project CreateProject()
  {
    return new Project("Proyecto Atlas", "Backend for project documentation based on markdown", "https://github.com/matigaleanodev/proyecto-atlas-api", "#1E293B");
  }

  private static Milestone CreateMilestone(Guid projectId, string title)
  {
    return new Milestone(projectId, title, "Cerrar la primera entrega publica.", MilestoneStatus.Planned);
  }

  private static Feature CreateFeature(Guid projectId, string title)
  {
    return new Feature(projectId, title, "Expose login endpoints.", FeatureStatus.Planned);
  }
}
