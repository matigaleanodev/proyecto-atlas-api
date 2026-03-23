using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ProyectoAtlas.Application.Projects;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProyectoAtlas.Api.Tests;

public class ApiIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory.WithWebHostBuilder(builder =>
    {
        builder.UseEnvironment("Development");
    });

    [Fact]
    public async Task GetOpenApiDocument_ShouldReturnOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
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
        var input = new CreateProjectInput(
            "Proyecto Atlas",
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
}
