using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Projects.Update;

public class UpdateProjectCommandHandlerTests
{
  [Fact]
  public async Task Execute_ShouldUpdateProvidedFieldsAndReturnProject()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    UpdateProjectCommandHandler useCase = new UpdateProjectCommandHandler(projectRepository);
    UpdateProjectCommand input = new UpdateProjectCommand(
        "Proyecto Atlas API",
        "Updated backend for project documentation",
        "https://github.com/matigaleanodev/proyecto-atlas-api-updated",
        "#0F172A");

    Project result = await useCase.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.Description, result.Description);
    Assert.Equal(input.RepositoryUrl, result.RepositoryUrl);
    Assert.Equal(input.Color, result.Color);
    Assert.Equal("proyecto-atlas-api", result.Slug);
    Assert.Same(result, projectRepository.UpdatedProject);
  }

  [Fact]
  public async Task Execute_ShouldRecalculateSlug_WhenTitleChanges()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    UpdateProjectCommandHandler useCase = new UpdateProjectCommandHandler(projectRepository);
    UpdateProjectCommand input = new UpdateProjectCommand("Atlas Platform", null, null, null);

    Project result = await useCase.Execute("proyecto-atlas", input);

    Assert.Equal("atlas-platform", result.Slug);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    UpdateProjectCommandHandler useCase = new UpdateProjectCommandHandler(new FakeProjectRepository());
    UpdateProjectCommand input = new UpdateProjectCommand("Atlas Platform", null, null, null);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() => useCase.Execute("missing-project", input));
  }

  [Fact]
  public async Task Execute_ShouldReplaceLinks_WhenLinksAreProvided()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B",
        [
          new ProjectLinkData("Repository", "https://github.com/matigaleanodev/proyecto-atlas-api", "Main source code", 1, ProjectLinkKind.Repository)
        ]);
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    UpdateProjectCommandHandler useCase = new UpdateProjectCommandHandler(projectRepository);
    UpdateProjectCommand input = new UpdateProjectCommand(
        "Proyecto Atlas API",
        null,
        null,
        null,
        [
          new UpdateProjectLink("Board", "https://linear.app/proyecto-atlas", "Work tracking board", 1, ProjectLinkKind.Board),
          new UpdateProjectLink("Monitoring", "https://grafana.example.com/atlas", "Operational dashboards", 2, ProjectLinkKind.Monitoring)
        ]);

    Project result = await useCase.Execute("proyecto-atlas", input);

    Assert.Equal(2, result.Links.Count);
    Assert.DoesNotContain(result.Links, link => link.Title == "Repository");
    ProjectLink boardLink = result.Links.OrderBy(link => link.SortOrder).First();
    Assert.Equal("Board", boardLink.Title);
    Assert.Equal("https://linear.app/proyecto-atlas", boardLink.Url);
    Assert.Equal("Work tracking board", boardLink.Description);
    Assert.Equal(ProjectLinkKind.Board, boardLink.Kind);
  }

  [Fact]
  public async Task Execute_ShouldPreserveLinks_WhenLinksAreNotProvided()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B",
        [
          new ProjectLinkData("Repository", "https://github.com/matigaleanodev/proyecto-atlas-api", "Main source code", 1, ProjectLinkKind.Repository)
        ]);
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    UpdateProjectCommandHandler useCase = new UpdateProjectCommandHandler(projectRepository);
    UpdateProjectCommand input = new UpdateProjectCommand(
        "Proyecto Atlas API",
        "Updated backend for project documentation",
        null,
        null);

    Project result = await useCase.Execute("proyecto-atlas", input);

    ProjectLink link = Assert.Single(result.Links);
    Assert.Equal("Repository", link.Title);
    Assert.Equal("https://github.com/matigaleanodev/proyecto-atlas-api", link.Url);
    Assert.Equal("Main source code", link.Description);
    Assert.Equal(1, link.SortOrder);
    Assert.Equal(ProjectLinkKind.Repository, link.Kind);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidProjectLinkItemException_WhenLinksContainDuplicateSortOrders()
  {
    Project existingProject = new Project(
        "Proyecto Atlas",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = existingProject
    };
    UpdateProjectCommandHandler useCase = new UpdateProjectCommandHandler(projectRepository);
    UpdateProjectCommand input = new UpdateProjectCommand(
        null,
        null,
        null,
        null,
        [
          new UpdateProjectLink("Repository", "https://github.com/matigaleanodev/proyecto-atlas-api", "Main source code", 1, ProjectLinkKind.Repository),
          new UpdateProjectLink("Board", "https://linear.app/proyecto-atlas", "Work tracking board", 1, ProjectLinkKind.Board)
        ]);

    await Assert.ThrowsAsync<InvalidProjectLinkItemException>(() => useCase.Execute("proyecto-atlas", input));
  }
}
