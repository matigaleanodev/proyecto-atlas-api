using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetSwaggerUi_ShouldReturnOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetHealth_ShouldReturnOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("ok", content);
    }

    [Fact]
    public async Task PostProjects_ShouldReturnCreatedProject()
    {
        var client = _factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var input = new CreateProjectInput(
            $"Proyecto Atlas {suffix}",
            "Backend for project documentation based on markdown",
            "https://github.com/matigaleanodev/proyecto-atlas-api",
            "#1E293B");

        var response = await client.PostAsJsonAsync("/projects", input);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        Assert.Equal(input.Title, root.GetProperty("title").GetString());
        Assert.Equal(input.Description, root.GetProperty("description").GetString());
        Assert.Equal(input.RepositoryUrl, root.GetProperty("repositoryUrl").GetString());
        Assert.Equal(input.Color, root.GetProperty("color").GetString());
        Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task GetProjects_ShouldReturnPagedProjects()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/projects?page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        Assert.Equal(1, root.GetProperty("page").GetInt32());
        Assert.Equal(2, root.GetProperty("pageSize").GetInt32());
        Assert.True(root.GetProperty("totalItems").GetInt32() >= 1);
        Assert.True(root.GetProperty("totalPages").GetInt32() >= 1);
        Assert.True(root.GetProperty("items").GetArrayLength() <= 2);
    }

    [Fact]
    public async Task GetProjects_ShouldFilterByQuery()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/projects?query=atlas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;
        var items = root.GetProperty("items");

        Assert.True(items.GetArrayLength() >= 1);

        foreach (var item in items.EnumerateArray())
        {
            var title = item.GetProperty("title").GetString() ?? string.Empty;
            var description = item.GetProperty("description").GetString() ?? string.Empty;

            Assert.True(
                title.Contains("atlas", StringComparison.OrdinalIgnoreCase) ||
                description.Contains("atlas", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task GetProjectBySlug_ShouldReturnProject_WhenSlugExists()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/projects/proyecto-atlas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        Assert.Equal("proyecto-atlas", root.GetProperty("slug").GetString());
        Assert.Equal("Proyecto Atlas", root.GetProperty("title").GetString());
    }

    [Fact]
    public async Task GetProjectBySlug_ShouldReturnNotFound_WhenSlugDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/projects/missing-project");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchProject_ShouldUpdateProject_WhenSlugExists()
    {
        var client = _factory.CreateClient();
        var input = new UpdateProjectInput(
            "Atlas Platform",
            "Updated backend for project documentation",
            "https://github.com/matigaleanodev/proyecto-atlas-platform",
            "#0F172A");

        var response = await client.PatchAsJsonAsync("/projects/proyecto-atlas", input);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        Assert.Equal(input.Title, root.GetProperty("title").GetString());
        Assert.Equal(input.Description, root.GetProperty("description").GetString());
        Assert.Equal(input.RepositoryUrl, root.GetProperty("repositoryUrl").GetString());
        Assert.Equal(input.Color, root.GetProperty("color").GetString());
        Assert.Equal("atlas-platform", root.GetProperty("slug").GetString());
    }

    [Fact]
    public async Task PatchProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
    {
        var client = _factory.CreateClient();
        var input = new UpdateProjectInput(
            "Atlas Platform",
            null,
            null,
            null);

        var response = await client.PatchAsJsonAsync("/projects/missing-project", input);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNoContent_WhenSlugExists()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/projects/proyecto-atlas");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNotFound_WhenSlugDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/projects/missing-project");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
