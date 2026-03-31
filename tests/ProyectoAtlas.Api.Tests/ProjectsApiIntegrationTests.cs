using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectsApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostProjects_ShouldReturnCreatedProject()
  {
    HttpClient client = Factory.CreateClient();
    string suffix = Guid.NewGuid().ToString("N")[..8];
    CreateProjectCommand input = new(
        $"Proyecto Atlas {suffix}",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.NotNull(response.Headers.Location);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.Description, root.GetProperty("description").GetString());
    Assert.Equal(input.RepositoryUrl, root.GetProperty("repositoryUrl").GetString());
    Assert.Equal(input.Color, root.GetProperty("color").GetString());
    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostProjects_ShouldReturnCreatedProjectWithLinks()
  {
    HttpClient client = Factory.CreateClient();
    string suffix = Guid.NewGuid().ToString("N")[..8];
    CreateProjectCommand input = new(
        $"Proyecto Atlas {suffix}",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B",
        [
          new CreateProjectLink("Repository", "https://github.com/matigaleanodev/proyecto-atlas-api", "Main source code", 2, ProjectLinkKind.Repository),
          new CreateProjectLink("Board", "https://linear.app/proyecto-atlas", "Work tracking board", 1, ProjectLinkKind.Board)
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    JsonElement[] links = root.GetProperty("links")
        .EnumerateArray()
        .OrderBy(link => link.GetProperty("sortOrder").GetInt32())
        .ToArray();
    Assert.Equal(2, links.Length);
    Assert.Equal("Board", links[0].GetProperty("title").GetString());
    Assert.Equal("https://linear.app/proyecto-atlas", links[0].GetProperty("url").GetString());
    Assert.Equal("Work tracking board", links[0].GetProperty("description").GetString());
    Assert.Equal(1, links[0].GetProperty("sortOrder").GetInt32());
    Assert.Equal("Board", links[0].GetProperty("kind").GetString());
  }

  [Fact]
  public async Task PostProjects_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectCommand input = new(
        "Átlas API: Guía / Inicial",
        "Backend for project documentation based on markdown",
        "https://github.com/matigaleanodev/proyecto-atlas-api",
        "#1E293B");

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("atlas-api-guia-inicial", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PostProjects_ShouldReturnConflict_WhenSlugAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectCommand input = new(
        "Proyecto Atlas",
        "Duplicate backend for project documentation based on markdown",
        "https://github.com/example/proyecto-atlas",
        "#111827");

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.ProjectSlugConflict, "Project slug");
  }

  [Fact]
  public async Task PostProjects_ShouldReturnBadRequest_WhenLinksContainDuplicateSortOrders()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectCommand input = new(
        "Proyecto Atlas New",
        "Backend for project documentation based on markdown",
        "https://github.com/example/proyecto-atlas-new",
        "#111827",
        [
          new CreateProjectLink("Repository", "https://github.com/example/proyecto-atlas-new", "Main source code", 1, ProjectLinkKind.Repository),
          new CreateProjectLink("Board", "https://linear.app/proyecto-atlas-new", "Work tracking board", 1, ProjectLinkKind.Board)
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ProjectLinkItemInvalid, "duplicate sort order");
  }

  [Fact]
  public async Task GetProjects_ShouldReturnPagedProjects()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects?page=1&pageSize=2");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(1, root.GetProperty("page").GetInt32());
    Assert.Equal(2, root.GetProperty("pageSize").GetInt32());
    Assert.True(root.GetProperty("totalItems").GetInt32() >= 1);
    Assert.True(root.GetProperty("totalPages").GetInt32() >= 1);
    Assert.True(root.GetProperty("items").GetArrayLength() <= 2);
    JsonElement firstProject = root.GetProperty("items")[0];
    Assert.True(firstProject.TryGetProperty("links", out JsonElement links));
    Assert.True(links.ValueKind == JsonValueKind.Array);
  }

  [Fact]
  public async Task GetProjects_ShouldFilterByQuery()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects?query=atlas");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.True(items.GetArrayLength() >= 1);

    foreach (JsonElement item in items.EnumerateArray())
    {
      string title = item.GetProperty("title").GetString() ?? string.Empty;
      string description = item.GetProperty("description").GetString() ?? string.Empty;

      Assert.True(
          title.Contains("atlas", StringComparison.OrdinalIgnoreCase) ||
          description.Contains("atlas", StringComparison.OrdinalIgnoreCase));
    }
  }

  [Fact]
  public async Task GetProjectBySlug_ShouldReturnProject_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("proyecto-atlas", root.GetProperty("slug").GetString());
    Assert.Equal("Proyecto Atlas", root.GetProperty("title").GetString());
    string[] linkTitles = root.GetProperty("links")
        .EnumerateArray()
        .Select(link => link.GetProperty("title").GetString() ?? string.Empty)
        .OrderBy(title => title, StringComparer.Ordinal)
        .ToArray();
    Assert.Equal(["Board", "Repository"], linkTitles);
  }

  [Fact]
  public async Task GetProjectBySlug_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PatchProject_ShouldUpdateProject_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        "Atlas Platform",
        "Updated backend for project documentation",
        "https://github.com/matigaleanodev/proyecto-atlas-platform",
        "#0F172A");

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.Description, root.GetProperty("description").GetString());
    Assert.Equal(input.RepositoryUrl, root.GetProperty("repositoryUrl").GetString());
    Assert.Equal(input.Color, root.GetProperty("color").GetString());
    Assert.Equal("atlas-platform", root.GetProperty("slug").GetString());
    Assert.Equal(2, root.GetProperty("links").GetArrayLength());
  }

  [Fact]
  public async Task PatchProject_ShouldReplaceLinks_WhenLinksAreProvided()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        "Atlas Platform",
        null,
        null,
        null,
        [
          new UpdateProjectLink("Monitoring", "https://grafana.example.com/atlas", "Operational dashboards", 1, ProjectLinkKind.Monitoring),
          new UpdateProjectLink("Environment", "https://vercel.example.com/atlas", "Deployment environment", 2, ProjectLinkKind.Environment)
        ]);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement[] links = root.GetProperty("links").EnumerateArray().ToArray();

    Assert.Equal(2, links.Length);
    Assert.Equal("Monitoring", links[0].GetProperty("title").GetString());
    Assert.Equal("https://grafana.example.com/atlas", links[0].GetProperty("url").GetString());
    Assert.Equal("Operational dashboards", links[0].GetProperty("description").GetString());
    Assert.Equal("Monitoring", links[0].GetProperty("kind").GetString());
    Assert.DoesNotContain(links, link => link.GetProperty("title").GetString() == "Repository");
  }

  [Fact]
  public async Task PatchProject_ShouldPreserveLinks_WhenLinksAreNotProvided()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        "Atlas Platform",
        "Updated backend for project documentation",
        null,
        null);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    string[] linkTitles = root.GetProperty("links")
        .EnumerateArray()
        .Select(link => link.GetProperty("title").GetString() ?? string.Empty)
        .OrderBy(title => title, StringComparer.Ordinal)
        .ToArray();

    Assert.Equal(["Board", "Repository"], linkTitles);
  }

  [Fact]
  public async Task PatchProject_ShouldReturnBadRequest_WhenLinksContainDuplicateSortOrders()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        null,
        null,
        null,
        null,
        [
          new UpdateProjectLink("Repository", "https://github.com/matigaleanodev/proyecto-atlas-api", "Main source code", 1, ProjectLinkKind.Repository),
          new UpdateProjectLink("Board", "https://linear.app/proyecto-atlas", "Work tracking board", 1, ProjectLinkKind.Board)
        ]);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ProjectLinkItemInvalid, "duplicate sort order");
  }

  [Fact]
  public async Task PatchProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        "Atlas Platform",
        null,
        null,
        null);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/missing-project", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PatchProject_ShouldReturnConflict_WhenSlugAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        "Proyecto Atlas",
        null,
        null,
        null);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/atlas-docs", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.ProjectSlugConflict, "Project slug");
  }

  [Fact]
  public async Task DeleteProject_ShouldReturnNoContent_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/proyecto-atlas");

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task DeleteProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/missing-project");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PostProjects_ShouldReturnValidationError_WhenPayloadIsInvalid()
  {
    HttpClient client = Factory.CreateClient();
    object input = new
    {
      title = (string?)null,
      description = "Backend for project documentation based on markdown",
      repositoryUrl = "https://github.com/example/proyecto-atlas",
      color = "#111827"
    };

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task GetUnknownError_ShouldReturnInternalServerErrorContract()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/unexpected-project");

    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.InternalServerError, AtlasErrorCodes.InternalServerError, "unexpected error");
  }
}
