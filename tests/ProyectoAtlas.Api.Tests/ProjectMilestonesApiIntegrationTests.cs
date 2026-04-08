using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectMilestonesApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostMilestones_ShouldReturnCreatedMilestone()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectMilestoneCommand input = new(
        "GA Release",
        "Cerrar la entrega general.",
        MilestoneStatus.Planned,
        new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/milestones", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.Summary, root.GetProperty("summary").GetString());
    Assert.Equal("Planned", root.GetProperty("status").GetString());
    Assert.Equal("ga-release", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PostMilestones_ShouldReturnConflict_WhenSlugAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectMilestoneCommand input = new("MVP Release", "Duplicado dentro del proyecto.", MilestoneStatus.Planned);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/milestones", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.MilestoneSlugConflict, "Milestone slug");
  }

  [Fact]
  public async Task GetMilestones_ShouldReturnPagedMilestones()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/milestones?page=1&pageSize=2");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(1, root.GetProperty("page").GetInt32());
    Assert.Equal(2, root.GetProperty("pageSize").GetInt32());
    Assert.True(root.GetProperty("items").GetArrayLength() <= 2);
  }

  [Fact]
  public async Task GetMilestones_ShouldFilterByStatus()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/milestones?status=Planned");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.True(items.GetArrayLength() >= 1);

    foreach (JsonElement item in items.EnumerateArray())
    {
      Assert.Equal("Planned", item.GetProperty("status").GetString());
    }
  }

  [Fact]
  public async Task GetMilestoneBySlug_ShouldReturnMilestone_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/milestones/mvp-release");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("MVP Release", root.GetProperty("title").GetString());
    Assert.Equal("mvp-release", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PatchMilestone_ShouldUpdateMilestone_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectMilestoneCommand input = new(
        "MVP Release",
        "Cerrar la primera entrega publica con onboarding completo.",
        MilestoneStatus.InProgress,
        new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc));

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas/milestones/mvp-release", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Summary, root.GetProperty("summary").GetString());
    Assert.Equal("InProgress", root.GetProperty("status").GetString());
  }

  [Fact]
  public async Task PatchMilestone_ShouldClearTargetDate_WhenRequested()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectMilestoneCommand input = new(null, null, null, null, true);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas/milestones/mvp-release", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(JsonValueKind.Null, root.GetProperty("targetDateUtc").ValueKind);
  }

  [Fact]
  public async Task DeleteMilestone_ShouldReturnNoContent_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/proyecto-atlas/milestones/mvp-release");

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task DeleteMilestone_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/proyecto-atlas/milestones/missing-milestone");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.MilestoneNotFound, "Milestone with slug");
  }
}
