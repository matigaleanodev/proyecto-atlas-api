using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class DocumentationActivityFeedApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task GetDocumentationActivityFeed_ShouldCombineAuditVersionsAndRelations()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage createDocumentationResponse = await client.PostAsJsonAsync(
        "/projects/proyecto-atlas/documentations",
        new CreateProjectDocumentationCommand(
            "API Overview",
            "# API",
            3,
            DocumentationKind.Note,
            DocumentationStatus.Draft,
            DocumentationArea.Backend));

    Assert.Equal(HttpStatusCode.Created, createDocumentationResponse.StatusCode);

    HttpResponseMessage createRelationResponse = await client.PostAsJsonAsync(
        "/projects/proyecto-atlas/documentations/api-overview/relations",
        new CreateDocumentationRelationCommand("getting-started", DocumentationRelationKind.RelatedTo));

    Assert.Equal(HttpStatusCode.Created, createRelationResponse.StatusCode);

    HttpResponseMessage patchResponse = await client.PatchAsJsonAsync(
        "/projects/proyecto-atlas/documentations/getting-started",
        new UpdateProjectDocumentationCommand(
            "Quick Start",
            "## Updated",
            2,
            DocumentationStatus.Published));

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    HttpResponseMessage response = await client.GetAsync(
        "/projects/proyecto-atlas/documentations/quick-start/activity-feed");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.True(items.GetArrayLength() >= 4);
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "AuditEvent");
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "VersionCreated");
    Assert.Contains(items.EnumerateArray(), item =>
        item.GetProperty("type").GetString() == "RelationCreated" &&
        item.GetProperty("relationDirection").GetString() == "Outgoing");
    Assert.Contains(items.EnumerateArray(), item =>
        item.GetProperty("type").GetString() == "RelationCreated" &&
        item.GetProperty("relationDirection").GetString() == "Incoming");
  }

  [Fact]
  public async Task GetDocumentationActivityFeed_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync(
        "/projects/proyecto-atlas/documentations/missing-doc/activity-feed");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, ProyectoAtlas.Application.Errors.AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }
}
