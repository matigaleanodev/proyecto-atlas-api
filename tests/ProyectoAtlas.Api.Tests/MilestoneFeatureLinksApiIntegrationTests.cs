using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class MilestoneFeatureLinksApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostMilestoneFeatureLinks_ShouldReturnCreatedLink()
  {
    HttpClient client = Factory.CreateClient();
    CreateMilestoneFeatureLinkCommand input = new("documentation-search");

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/milestones/mvp-release/features", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    Assert.NotEqual(Guid.Empty, jsonDocument.RootElement.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostMilestoneFeatureLinks_ShouldReturnConflict_WhenLinkAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateMilestoneFeatureLinkCommand input = new("authentication-api");

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/milestones/mvp-release/features", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.MilestoneFeatureLinkConflict, "already exists");
  }

  [Fact]
  public async Task GetMilestoneFeatureLinks_ShouldReturnLinks()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/milestones/mvp-release/features");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task GetFeatureMilestoneLinks_ShouldReturnLinks()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/features/authentication-api/milestones");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task DeleteMilestoneFeatureLink_ShouldReturnNoContent_WhenLinkExists()
  {
    HttpClient client = Factory.CreateClient();
    HttpResponseMessage listResponse =
        await client.GetAsync("/projects/proyecto-atlas/milestones/mvp-release/features");

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);
    Guid linkId = listDocument.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

    HttpResponseMessage response =
        await client.DeleteAsync($"/projects/proyecto-atlas/milestones/mvp-release/features/{linkId}");

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }
}
