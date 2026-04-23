using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class FeatureDocumentationLinksApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostFeatureDocumentationLinks_ShouldReturnCreatedLink()
  {
    HttpClient client = Factory.CreateClient();
    CreateFeatureDocumentationLinkCommand input = new("adr-001-architecture");

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/features/authentication-api/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
    Assert.NotEqual(Guid.Empty, root.GetProperty("projectId").GetGuid());
    Assert.NotEqual(Guid.Empty, root.GetProperty("featureId").GetGuid());
    Assert.NotEqual(Guid.Empty, root.GetProperty("documentationId").GetGuid());
  }

  [Fact]
  public async Task PostFeatureDocumentationLinks_ShouldReturnConflict_WhenLinkAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateFeatureDocumentationLinkCommand input = new("getting-started");

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/features/authentication-api/documentations", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.FeatureDocumentationLinkConflict, "already exists");
  }

  [Fact]
  public async Task GetFeatureDocumentationLinks_ShouldReturnFeatureLinks()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/features/authentication-api/documentations");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Single(items.EnumerateArray());
  }

  [Fact]
  public async Task GetDocumentationFeatureLinks_ShouldReturnDocumentationLinks()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/features");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Single(items.EnumerateArray());
  }

  [Fact]
  public async Task DeleteFeatureDocumentationLink_ShouldReturnNoContent_WhenLinkExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage listResponse =
        await client.GetAsync("/projects/proyecto-atlas/features/authentication-api/documentations");

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);
    Guid linkId = listDocument.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

    HttpResponseMessage deleteResponse =
        await client.DeleteAsync($"/projects/proyecto-atlas/features/authentication-api/documentations/{linkId}");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteFeatureDocumentationLink_ShouldReturnNotFound_WhenFeatureDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.DeleteAsync($"/projects/proyecto-atlas/features/missing-feature/documentations/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.FeatureNotFound, "Feature with slug");
  }
}
