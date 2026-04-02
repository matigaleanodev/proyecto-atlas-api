using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class DocumentationRelationsApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostDocumentationRelations_ShouldReturnCreatedRelation()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectDocumentationCommand createDocumentationInput = new(
        "API Overview",
        "# API",
        3,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);
    HttpResponseMessage createDocumentationResponse =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", createDocumentationInput);

    Assert.Equal(HttpStatusCode.Created, createDocumentationResponse.StatusCode);

    CreateDocumentationRelationCommand input = new("api-overview", DocumentationRelationKind.RelatedTo);

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started/relations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("RelatedTo", root.GetProperty("kind").GetString());
    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostDocumentationRelations_ShouldReturnBadRequest_WhenRelationIsSelfReferential()
  {
    HttpClient client = Factory.CreateClient();
    CreateDocumentationRelationCommand input = new("getting-started", DocumentationRelationKind.RelatedTo);

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started/relations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.DocumentationRelationInvalid, "same documentation");
  }

  [Fact]
  public async Task PostDocumentationRelations_ShouldReturnConflict_WhenRelationAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateDocumentationRelationCommand input = new("adr-001-architecture", DocumentationRelationKind.Implements);

    HttpResponseMessage response =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started/relations", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.DocumentationRelationConflict, "already exists");
  }

  [Fact]
  public async Task GetDocumentationRelations_ShouldReturnOutgoingRelations()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/relations");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Single(items.EnumerateArray());
    Assert.Equal("Implements", items[0].GetProperty("kind").GetString());
  }

  [Fact]
  public async Task DeleteDocumentationRelation_ShouldReturnNoContent_WhenRelationExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage listResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/relations");

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);
    Guid relationId = listDocument.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

    HttpResponseMessage deleteResponse =
        await client.DeleteAsync($"/projects/proyecto-atlas/documentations/getting-started/relations/{relationId}");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteDocumentationRelation_ShouldReturnNotFound_WhenRelationDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.DeleteAsync($"/projects/proyecto-atlas/documentations/getting-started/relations/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationRelationNotFound, "Documentation relation");
  }
}
