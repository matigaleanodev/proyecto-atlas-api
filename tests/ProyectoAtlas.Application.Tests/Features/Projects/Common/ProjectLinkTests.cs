using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Projects.Common;

public class ProjectLinkTests
{
  [Fact]
  public void Constructor_ShouldAssignLinks_WhenLinksAreValid()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B",
        [
          new ProjectLinkData(
              "Repository",
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              "Main source repository",
              2,
              ProjectLinkKind.Repository),
          new ProjectLinkData(
              "Docs",
              "https://docs.example.com/atlas",
              "External documentation",
              1,
              ProjectLinkKind.Documentation)
        ]);

    Assert.Equal(2, project.Links.Count);
    Assert.Equal("Docs", project.Links.First().Title);
    Assert.Equal("Repository", project.Links.Last().Title);
  }

  [Fact]
  public void Constructor_ShouldThrowInvalidProjectLinkListException_WhenSortOrderIsDuplicated()
  {
    Action action = () => _ = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B",
        [
          new ProjectLinkData(
              "Repository",
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              "Main source repository",
              1,
              ProjectLinkKind.Repository),
          new ProjectLinkData(
              "Docs",
              "https://docs.example.com/atlas",
              "External documentation",
              1,
              ProjectLinkKind.Documentation)
        ]);

    Assert.Throws<InvalidProjectLinkListException>(action);
  }

  [Fact]
  public void ReplaceLinks_ShouldReplaceCompleteCollection_WhenLinksAreValid()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B",
        [
          new ProjectLinkData(
              "Repository",
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              "Main source repository",
              1,
              ProjectLinkKind.Repository)
        ]);

    project.ReplaceLinks(
        [
          new ProjectLinkData(
              "Board",
              "https://linear.app/atlas",
              "Planning board",
              1,
              ProjectLinkKind.Board),
          new ProjectLinkData(
              "Monitoring",
              "https://grafana.example.com/atlas",
              "Operational dashboards",
              2,
              ProjectLinkKind.Monitoring)
        ]);

    Assert.Equal(2, project.Links.Count);
    Assert.DoesNotContain(project.Links, link => link.Kind == ProjectLinkKind.Repository);
    Assert.Contains(project.Links, link => link.Kind == ProjectLinkKind.Board);
    Assert.Contains(project.Links, link => link.Kind == ProjectLinkKind.Monitoring);
  }

  [Fact]
  public void ReplaceLinks_ShouldThrowInvalidProjectLinkListException_WhenSortOrderIsInvalid()
  {
    Project project = new(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");

    Action action = () => project.ReplaceLinks(
        [
          new ProjectLinkData(
              "Repository",
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              "Main source repository",
              0,
              ProjectLinkKind.Repository)
        ]);

    Assert.Throws<InvalidProjectLinkListException>(action);
  }
}
