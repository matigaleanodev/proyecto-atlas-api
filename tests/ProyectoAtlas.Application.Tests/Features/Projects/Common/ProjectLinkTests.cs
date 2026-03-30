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
              2,
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              ProjectLinkKind.Repository,
              "Main source repository"),
          new ProjectLinkData(
              "Docs",
              1,
              "https://docs.example.com/atlas",
              ProjectLinkKind.Documentation,
              "External documentation")
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
              1,
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              ProjectLinkKind.Repository,
              "Main source repository"),
          new ProjectLinkData(
              "Docs",
              1,
              "https://docs.example.com/atlas",
              ProjectLinkKind.Documentation,
              "External documentation")
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
              1,
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              ProjectLinkKind.Repository,
              "Main source repository")
        ]);

    project.ReplaceLinks(
        [
          new ProjectLinkData(
              "Board",
              1,
              "https://linear.app/atlas",
              ProjectLinkKind.Board,
              "Planning board"),
          new ProjectLinkData(
              "Monitoring",
              2,
              "https://grafana.example.com/atlas",
              ProjectLinkKind.Monitoring,
              "Operational dashboards")
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
              0,
              "https://github.com/matigaleanodev/proyecto-atlas-api",
              ProjectLinkKind.Repository,
              "Main source repository")
        ]);

    Assert.Throws<InvalidProjectLinkListException>(action);
  }
}
