using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class DocumentationVersionsApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PatchProjectDocumentation_ShouldCreateVersion_WhenVersionedFieldsChange()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        Domain.Documentations.DocumentationStatus.Published);

    HttpResponseMessage patchResponse =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    string patchContent = await patchResponse.Content.ReadAsStringAsync();
    using JsonDocument patchDocument = JsonDocument.Parse(patchContent);
    string updatedSlug = patchDocument.RootElement.GetProperty("slug").GetString() ?? string.Empty;

    HttpResponseMessage listResponse =
        await client.GetAsync($"/projects/proyecto-atlas/documentations/{updatedSlug}/versions");

    Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);
    JsonElement item = Assert.Single(listDocument.RootElement.GetProperty("items").EnumerateArray());

    Assert.Equal(1, item.GetProperty("versionNumber").GetInt32());
    Assert.Equal("Getting Started", item.GetProperty("title").GetString());
    Assert.Equal("# Proyecto Atlas", item.GetProperty("contentMarkdown").GetString());
    Assert.Equal("Draft", item.GetProperty("status").GetString());
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldNotCreateVersion_WhenOnlySortOrderChanges()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        null,
        null,
        7,
        null);

    HttpResponseMessage patchResponse =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    HttpResponseMessage listResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/versions");

    Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

    string listContent = await listResponse.Content.ReadAsStringAsync();
    using JsonDocument listDocument = JsonDocument.Parse(listContent);

    Assert.Empty(listDocument.RootElement.GetProperty("items").EnumerateArray());
  }

  [Fact]
  public async Task GetDocumentationVersionByNumber_ShouldReturnVersion_WhenVersionExists()
  {
    HttpClient client = Factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        Domain.Documentations.DocumentationStatus.Published);

    HttpResponseMessage patchResponse =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    string patchContent = await patchResponse.Content.ReadAsStringAsync();
    using JsonDocument patchDocument = JsonDocument.Parse(patchContent);
    string updatedSlug = patchDocument.RootElement.GetProperty("slug").GetString() ?? string.Empty;

    HttpResponseMessage response =
        await client.GetAsync($"/projects/proyecto-atlas/documentations/{updatedSlug}/versions/1");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(1, root.GetProperty("versionNumber").GetInt32());
    Assert.Equal("Getting Started", root.GetProperty("title").GetString());
    Assert.Equal("# Proyecto Atlas", root.GetProperty("contentMarkdown").GetString());
    Assert.Equal("Draft", root.GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetDocumentationVersionByNumber_ShouldReturnNotFound_WhenVersionDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started/versions/1");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationVersionNotFound, "version");
  }
}
