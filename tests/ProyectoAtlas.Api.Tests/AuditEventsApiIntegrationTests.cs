using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class AuditEventsApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task GetProjectAuditEvents_ShouldReturnEventsAfterProjectUpdate()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand input = new(
        "Atlas Platform",
        "Updated backend for project documentation",
        "https://github.com/matigaleanodev/proyecto-atlas-platform",
        "#0F172A");

    HttpResponseMessage patchResponse = await client.PatchAsJsonAsync("/projects/proyecto-atlas", input);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    HttpResponseMessage getResponse = await client.GetAsync("/projects/atlas-platform/audit-events");

    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

    string content = await getResponse.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.True(items.GetArrayLength() >= 1);
    Assert.Equal("Project", items[0].GetProperty("entityType").GetString());
    Assert.Equal("Updated", items[0].GetProperty("action").GetString());
  }

  [Fact]
  public async Task GetDocumentationAuditEvents_ShouldReturnEventsAfterDocumentationUpdate()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        2,
        DocumentationStatus.Published);

    HttpResponseMessage patchResponse =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    HttpResponseMessage getResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/quick-start/audit-events");

    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

    string content = await getResponse.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.True(items.GetArrayLength() >= 1);
    Assert.Equal("Documentation", items[0].GetProperty("entityType").GetString());
    Assert.Equal("Updated", items[0].GetProperty("action").GetString());
  }

  [Fact]
  public async Task GetDocumentationAuditEvents_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations/missing-doc/audit-events");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, ProyectoAtlas.Application.Errors.AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

  [Fact]
  public async Task GetProjectAuditEvents_ShouldFilterByActionAndLimit()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectCommand updateInput = new(
        "Atlas Platform",
        "Updated backend for project documentation",
        "https://github.com/matigaleanodev/proyecto-atlas-platform",
        "#0F172A");

    HttpResponseMessage patchResponse = await client.PatchAsJsonAsync("/projects/proyecto-atlas", updateInput);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    HttpResponseMessage getResponse = await client.GetAsync(
        $"/projects/atlas-platform/audit-events?action={AuditAction.Updated}&limit=1");

    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

    string content = await getResponse.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("Updated", items[0].GetProperty("action").GetString());
  }

  [Fact]
  public async Task GetDocumentationAuditEvents_ShouldReturnBadRequest_WhenDateRangeIsInvalid()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync(
        "/projects/proyecto-atlas/documentations/getting-started/audit-events?occurredFromUtc=2026-05-13T00:00:00Z&occurredToUtc=2026-05-12T00:00:00Z");

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        ProyectoAtlas.Application.Errors.AtlasErrorCodes.ValidationError,
        "occurredFromUtc");
  }
}
