using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectOverviewApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task GetProjectOverview_ShouldReturnCountsAndRecentActivity()
  {
    HttpClient client = Factory.CreateClient();

    await client.PatchAsJsonAsync(
        "/projects/proyecto-atlas",
        new UpdateProjectCommand(
            "Atlas Platform",
            "Updated backend for project documentation",
            "https://github.com/matigaleanodev/proyecto-atlas-platform",
            "#0F172A"));

    await client.PatchAsJsonAsync(
        "/projects/atlas-platform/documentations/getting-started",
        new UpdateProjectDocumentationCommand(
            "Quick Start",
            "## Updated",
            2,
            DocumentationStatus.Published));

    HttpResponseMessage response = await client.GetAsync("/projects/atlas-platform/overview");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("atlas-platform", root.GetProperty("projectSlug").GetString());
    Assert.Equal("Atlas Platform", root.GetProperty("projectTitle").GetString());
    Assert.Equal(2, root.GetProperty("documentationCount").GetInt32());
    Assert.Equal(2, root.GetProperty("featureCount").GetInt32());
    Assert.Equal(2, root.GetProperty("milestoneCount").GetInt32());
    Assert.Equal(1, root.GetProperty("outgoingRelationCount").GetInt32());
    Assert.Equal(0, root.GetProperty("incomingRelationCount").GetInt32());
    Assert.True(root.GetProperty("recentActivity").GetArrayLength() >= 2);
  }

  [Fact]
  public async Task GetProjectOverview_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/overview");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, ProyectoAtlas.Application.Errors.AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }
}
