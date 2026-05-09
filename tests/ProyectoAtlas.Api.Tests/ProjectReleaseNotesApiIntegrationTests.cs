using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Application.Features.ReleaseNotes.Create;
using ProyectoAtlas.Application.Features.ReleaseNotes.Update;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectReleaseNotesApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task PostReleaseNotes_ShouldCreateReleaseNotes_WithForcedKind()
  {
    HttpClient client = Factory.CreateClient();
    CreateProjectReleaseNotesCommand input = new(
        "Release 1.0.0",
        "## Highlights",
        1,
        DocumentationStatus.Published,
        DocumentationArea.Product);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/release-notes", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);

    Assert.Equal("ReleaseNotes", jsonDocument.RootElement.GetProperty("kind").GetString());
    Assert.Equal("release-1-0-0", jsonDocument.RootElement.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task GetReleaseNotes_ShouldReturnOnlyReleaseNotes()
  {
    HttpClient client = Factory.CreateClient();
    await CreateReleaseNotes(client, "Release 1.0.0");
    await CreateReleaseNotes(client, "Release 1.1.0");

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/release-notes");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.Equal(2, items.GetArrayLength());
    Assert.All(items.EnumerateArray().ToArray(), item => Assert.Equal("ReleaseNotes", item.GetProperty("kind").GetString()));
  }

  [Fact]
  public async Task GetReleaseNotesBySlug_ShouldReturnNotFound_WhenSlugBelongsToAnotherDocumentationKind()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/release-notes/getting-started");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

  [Fact]
  public async Task PatchReleaseNotes_ShouldUpdateReleaseNotes()
  {
    HttpClient client = Factory.CreateClient();
    await CreateReleaseNotes(client, "Release 1.0.0");
    UpdateProjectReleaseNotesCommand input = new(
        "Release 1.0.1",
        "## Patch",
        2,
        DocumentationStatus.Published);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/release-notes/release-1-0-0", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);

    Assert.Equal("Release 1.0.1", jsonDocument.RootElement.GetProperty("title").GetString());
    Assert.Equal("ReleaseNotes", jsonDocument.RootElement.GetProperty("kind").GetString());
  }

  [Fact]
  public async Task DeleteReleaseNotes_ShouldDeleteReleaseNotes()
  {
    HttpClient client = Factory.CreateClient();
    await CreateReleaseNotes(client, "Release 1.0.0");

    HttpResponseMessage deleteResponse =
        await client.DeleteAsync("/projects/proyecto-atlas/release-notes/release-1-0-0");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }

  private static async Task CreateReleaseNotes(HttpClient client, string title)
  {
    CreateProjectReleaseNotesCommand input = new(
        title,
        "## Highlights",
        1,
        DocumentationStatus.Published,
        DocumentationArea.Product);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/release-notes", input);
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
  }
}
