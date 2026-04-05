using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class DocumentationResourcesApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostDocumentationResources_ShouldReturnCreatedResource()
  {
    HttpClient client = Factory.CreateClient();
    CreateDocumentationResourceCommand input = new("Architecture Board", "https://miro.com/app/board/uXj123", DocumentationResourceKind.Design);

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started/resources", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.Url, root.GetProperty("url").GetString());
    Assert.Equal("Design", root.GetProperty("kind").GetString());
  }

  [Fact]
  public async Task PostDocumentationResources_ShouldReturnBadRequest_WhenUrlIsInvalid()
  {
    HttpClient client = Factory.CreateClient();
    CreateDocumentationResourceCommand input = new("Architecture Board", "miro-board", DocumentationResourceKind.Design);

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started/resources", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.DocumentationResourceInvalid, "valid absolute");
  }

  [Fact]
  public async Task PostDocumentationResources_ShouldReturnConflict_WhenResourceAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateDocumentationResourceCommand input = new("OpenAPI Spec", "https://api.example.com/openapi.json", DocumentationResourceKind.ApiSpec);

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started/resources", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.DocumentationResourceConflict, "already exists");
  }

  [Fact]
  public async Task GetDocumentationResources_ShouldReturnResources()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/resources");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Single(items.EnumerateArray());
    Assert.Equal("OpenAPI Spec", items[0].GetProperty("title").GetString());
  }

  [Fact]
  public async Task DeleteDocumentationResource_ShouldReturnNoContent_WhenResourceExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage listResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/resources");

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);
    Guid resourceId = listDocument.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

    HttpResponseMessage deleteResponse =
        await client.DeleteAsync($"/projects/proyecto-atlas/documentations/getting-started/resources/{resourceId}");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteDocumentationResource_ShouldReturnNotFound_WhenResourceDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.DeleteAsync($"/projects/proyecto-atlas/documentations/getting-started/resources/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationResourceNotFound, "Documentation resource");
  }
}
