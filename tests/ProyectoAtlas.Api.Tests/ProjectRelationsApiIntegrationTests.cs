using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectRelationsApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostProjectRelations_ShouldReturnCreatedRelation()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectRelationCommand input = new("task-forge", ProjectRelationKind.DependsOn);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/relations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("DependsOn", root.GetProperty("kind").GetString());
    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostProjectRelations_ShouldReturnBadRequest_WhenRelationIsSelfReferential()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectRelationCommand input = new("proyecto-atlas", ProjectRelationKind.DependsOn);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/relations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ProjectRelationInvalid, "same project");
  }

  [Fact]
  public async Task PostProjectRelations_ShouldReturnConflict_WhenRelationAlreadyExists()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectRelationCommand input = new("atlas-docs", ProjectRelationKind.IntegratesWith);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/relations", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.ProjectRelationConflict, "already exists");
  }

  [Fact]
  public async Task GetProjectRelations_ShouldReturnOutgoingRelations()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/relations");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Single(items.EnumerateArray());
    Assert.Equal("IntegratesWith", items[0].GetProperty("kind").GetString());
  }

  [Fact]
  public async Task DeleteProjectRelation_ShouldReturnNoContent_WhenRelationExists()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage listResponse = await client.GetAsync("/projects/proyecto-atlas/relations");

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);
    Guid relationId = listDocument.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

    HttpResponseMessage deleteResponse = await client.DeleteAsync($"/projects/proyecto-atlas/relations/{relationId}");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteProjectRelation_ShouldReturnNotFound_WhenRelationDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.DeleteAsync($"/projects/proyecto-atlas/relations/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectRelationNotFound, "Project relation");
  }
}
