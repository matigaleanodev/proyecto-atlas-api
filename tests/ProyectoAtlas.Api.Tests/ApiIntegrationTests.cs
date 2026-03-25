using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

public class ApiIntegrationTests(ApiTestWebApplicationFactory factory) : IClassFixture<ApiTestWebApplicationFactory>, IAsyncLifetime
{
  private readonly ApiTestWebApplicationFactory _factory = factory;

  public async Task InitializeAsync()
  {
    await _factory.ResetDatabaseAsync();
    await _factory.SeedProjectsAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
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
  public async Task PostProjects_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
  {
    HttpClient client = _factory.CreateClient();
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
    HttpClient client = _factory.CreateClient();
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
  public async Task PostProjectDocumentations_ShouldReturnCreatedDocumentation()
  {
    HttpClient client = _factory.CreateClient();
    string suffix = Guid.NewGuid().ToString("N")[..8];
    CreateProjectDocumentationCommand input = new(
        $"Getting Started {suffix}",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft);

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
    Assert.Equal(input.Kind.ToString(), root.GetProperty("kind").GetString());
    Assert.Equal(input.Status.ToString(), root.GetProperty("status").GetString());
    Assert.Equal($"getting-started-{suffix.ToLowerInvariant()}", root.GetProperty("slug").GetString());
    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Guía API: sección / inicial",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("guia-api-seccion-inicial", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/missing-project/documentations", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnConflict_WhenSlugAlreadyExistsWithinProject()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Duplicate",
        3,
        DocumentationKind.Note,
        DocumentationStatus.Draft);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.DocumentationSlugConflict, "Documentation slug");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnCreated_WhenSlugExistsInAnotherProject()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Atlas Docs",
        2,
        DocumentationKind.Note,
        DocumentationStatus.Published);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/atlas-docs/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnValidationError_WhenKindIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    object input = new
    {
      title = "Getting Started",
      contentMarkdown = "# Proyecto Atlas",
      sortOrder = 1,
      kind = "InvalidKind"
    };

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnValidationError_WhenStatusIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    object input = new
    {
      title = "Getting Started",
      contentMarkdown = "# Proyecto Atlas",
      sortOrder = 1,
      kind = "Page",
      status = "InvalidStatus"
    };

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenDecisionTitleConventionIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Architecture decision without ADR prefix",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Decision,
        DocumentationStatus.Draft);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTitleConventionInvalid,
        "ADR-XXX");
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
    JsonElement item = Assert.Single(root.GetProperty("items").EnumerateArray());
    Assert.Equal("Page", item.GetProperty("kind").GetString());
    Assert.Equal("Draft", item.GetProperty("status").GetString());
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
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Decision", items[0].GetProperty("kind").GetString());
    Assert.Equal("Published", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByKind()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?kind=Decision");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Decision", items[0].GetProperty("kind").GetString());
    Assert.Equal("Published", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByStatus()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?status=Draft");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("Getting Started", items[0].GetProperty("title").GetString());
    Assert.Equal("Page", items[0].GetProperty("kind").GetString());
    Assert.Equal("Draft", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByKindAndStatus()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations?kind=Decision&status=Published");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Decision", items[0].GetProperty("kind").GetString());
    Assert.Equal("Published", items[0].GetProperty("status").GetString());
  }

  [Theory]
  [InlineData("/projects/proyecto-atlas/documentations?kind=InvalidKind")]
  [InlineData("/projects/proyecto-atlas/documentations?status=InvalidStatus")]
  public async Task GetProjectDocumentations_ShouldReturnValidationError_WhenFilterIsInvalid(string path)
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync(path);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/documentations");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
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
    Assert.Equal("Page", root.GetProperty("kind").GetString());
    Assert.Equal("Draft", root.GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations/missing-doc");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldUpdateDocumentation_WhenDocumentationExists()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Published);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.ContentMarkdown, root.GetProperty("contentMarkdown").GetString());
    Assert.Equal(input.SortOrder, root.GetProperty("sortOrder").GetInt32());
    Assert.Equal("Page", root.GetProperty("kind").GetString());
    Assert.Equal(input.Status.ToString(), root.GetProperty("status").GetString());
    Assert.Equal("quick-start", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnBadRequest_WhenDecisionTitleConventionIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Architecture without ADR prefix",
        "## Updated",
        3,
        DocumentationStatus.Published);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/adr-001-architecture", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTitleConventionInvalid,
        "ADR-XXX");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Draft);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/missing-doc", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnConflict_WhenSlugAlreadyExistsWithinProject()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand createCommand = new(
        "Release Checklist",
        "# Checklist",
        3,
        DocumentationKind.Note,
        DocumentationStatus.Draft);
    UpdateProjectDocumentationCommand input = new(
        "Getting Started",
        null,
        null,
        null);

    HttpResponseMessage createResponse =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", createCommand);

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/release-checklist", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.DocumentationSlugConflict, "Documentation slug");
  }

  [Fact]
  public async Task DeleteProjectDocumentation_ShouldReturnNoContent_WhenDocumentationExists()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage deleteResponse =
        await client.DeleteAsync("/projects/proyecto-atlas/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

    HttpResponseMessage getResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteProjectDocumentation_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response =
        await client.DeleteAsync("/projects/proyecto-atlas/documentations/missing-doc");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
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
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PatchProject_ShouldUpdateProject_WhenSlugExists()
  {
    HttpClient client = _factory.CreateClient();
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
  }

  [Fact]
  public async Task PatchProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
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
    HttpClient client = _factory.CreateClient();
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
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PostProjects_ShouldReturnValidationError_WhenPayloadIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
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
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/unexpected-project");

    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.InternalServerError, AtlasErrorCodes.InternalServerError, "unexpected error");
  }

  private static async Task AssertErrorResponse(
      HttpResponseMessage response,
      HttpStatusCode expectedStatusCode,
      string expectedCode,
      string expectedMessageFragment)
  {
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal((int)expectedStatusCode, root.GetProperty("statusCode").GetInt32());
    Assert.Equal(expectedCode, root.GetProperty("code").GetString());

    string message = root.GetProperty("message").GetString() ?? string.Empty;
    Assert.Contains(expectedMessageFragment, message, StringComparison.OrdinalIgnoreCase);
  }
}
