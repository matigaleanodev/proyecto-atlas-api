using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Application.Projects;

namespace ProyectoAtlas.Api.Tests;

public class ApiIntegrationTests(ApiTestWebApplicationFactory factory) : IClassFixture<ApiTestWebApplicationFactory>, IAsyncLifetime
{
  private readonly ApiTestWebApplicationFactory _factory = factory;

  public async Task InitializeAsync()
  {
    await _factory.ResetDatabaseAsync();
    await _factory.SeedProjectsAsync();
  }

  public async Task DisposeAsync()
  {
    await _factory.ResetDatabaseAsync();
  }

  [Fact]
  public async Task GetOpenApiDocument_ShouldReturnOk()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/openapi/v1.json");

    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
  }

  [Fact]
  public async Task GetSwaggerUi_ShouldReturnOk()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/swagger/index.html");

    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
  }

  [Fact]
  public async Task GetHealth_ShouldReturnOk()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/health");

    Assert.True(response.IsSuccessStatusCode);
    string content = await response.Content.ReadAsStringAsync();
    Assert.Contains("ok", content);
  }

  [Fact]
  public async Task PostProjects_ShouldReturnCreatedProject()
  {
    HttpClient client = _factory.CreateClient();
    string suffix = Guid.NewGuid().ToString("N")[..8];
    CreateProjectInput input = new CreateProjectInput(
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
  public async Task PostProjectDocumentations_ShouldReturnCreatedDocumentation()
  {
    HttpClient client = _factory.CreateClient();
    string suffix = Guid.NewGuid().ToString("N")[..8];
    CreateProjectDocumentationInput input = new CreateProjectDocumentationInput(
        $"Getting Started {suffix}",
        "# Proyecto Atlas",
        1);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.NotNull(response.Headers.Location);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.ContentMarkdown, root.GetProperty("contentMarkdown").GetString());
    Assert.Equal(input.SortOrder, root.GetProperty("sortOrder").GetInt32());
    Assert.Equal($"getting-started-{suffix.ToLowerInvariant()}", root.GetProperty("slug").GetString());
    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationInput input = new CreateProjectDocumentationInput(
        "Getting Started",
        "# Proyecto Atlas",
        1);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/missing-project/documentations", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldReturnPagedDocumentations()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?page=1&pageSize=1");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(1, root.GetProperty("page").GetInt32());
    Assert.Equal(1, root.GetProperty("pageSize").GetInt32());
    Assert.Equal(2, root.GetProperty("totalItems").GetInt32());
    Assert.Equal(2, root.GetProperty("totalPages").GetInt32());
    Assert.Single(root.GetProperty("items").EnumerateArray());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByQuery()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?query=arch");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("Architecture", items[0].GetProperty("title").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/documentations");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnDocumentation_WhenDocumentationExists()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("getting-started", root.GetProperty("slug").GetString());
    Assert.Equal("Getting Started", root.GetProperty("title").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations/missing-doc");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GetProjects_ShouldReturnPagedProjects()
  {
    HttpClient client = _factory.CreateClient();

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
  }

  [Fact]
  public async Task GetProjects_ShouldFilterByQuery()
  {
    HttpClient client = _factory.CreateClient();

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
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("proyecto-atlas", root.GetProperty("slug").GetString());
    Assert.Equal("Proyecto Atlas", root.GetProperty("title").GetString());
  }

  [Fact]
  public async Task GetProjectBySlug_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task PatchProject_ShouldUpdateProject_WhenSlugExists()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectInput input = new(
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
  }

  [Fact]
  public async Task PatchProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectInput input = new UpdateProjectInput(
        "Atlas Platform",
        null,
        null,
        null);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/missing-project", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task DeleteProject_ShouldReturnNoContent_WhenSlugExists()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/proyecto-atlas");

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task DeleteProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/missing-project");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}
