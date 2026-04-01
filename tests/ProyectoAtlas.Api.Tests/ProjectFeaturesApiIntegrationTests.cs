using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectFeaturesApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostFeatures_ShouldReturnCreatedFeature()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectFeatureCommand input = new("Notifications API", "Expose notification endpoints.", FeatureStatus.Planned);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/features", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.Summary, root.GetProperty("summary").GetString());
    Assert.Equal("Planned", root.GetProperty("status").GetString());
    Assert.Equal("notifications-api", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PostFeatures_ShouldReturnConflict_WhenSlugAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectFeatureCommand input = new("Authentication API", "Duplicate title inside the project.", FeatureStatus.Planned);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/features", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.FeatureSlugConflict, "Feature slug");
  }

  [Fact]
  public async Task GetFeatures_ShouldReturnPagedFeatures()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/features?page=1&pageSize=2");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(1, root.GetProperty("page").GetInt32());
    Assert.Equal(2, root.GetProperty("pageSize").GetInt32());
    Assert.True(root.GetProperty("items").GetArrayLength() <= 2);
  }

  [Fact]
  public async Task GetFeatures_ShouldFilterByStatus()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/features?status=Planned");

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
  public async Task GetFeatureBySlug_ShouldReturnFeature_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/features/authentication-api");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("Authentication API", root.GetProperty("title").GetString());
    Assert.Equal("authentication-api", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PatchFeature_ShouldUpdateFeature_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectFeatureCommand input = new("Authentication API", "Expose login and refresh endpoints.", FeatureStatus.InProgress);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas/features/authentication-api", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Summary, root.GetProperty("summary").GetString());
    Assert.Equal("InProgress", root.GetProperty("status").GetString());
  }

  [Fact]
  public async Task PatchFeature_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectFeatureCommand input = new("Missing", "Missing feature", FeatureStatus.Done);

    HttpResponseMessage response = await client.PatchAsJsonAsync("/projects/proyecto-atlas/features/missing-feature", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.FeatureNotFound, "Feature with slug");
  }

  [Fact]
  public async Task DeleteFeature_ShouldReturnNoContent_WhenSlugExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/proyecto-atlas/features/authentication-api");

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task DeleteFeature_ShouldReturnNotFound_WhenSlugDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync("/projects/proyecto-atlas/features/missing-feature");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.FeatureNotFound, "Feature with slug");
  }
}
