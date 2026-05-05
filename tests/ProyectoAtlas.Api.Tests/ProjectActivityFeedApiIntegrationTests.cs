using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectActivityFeedApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task GetProjectActivityFeed_ShouldReturnConsolidatedRecentChanges()
  {
    HttpClient client = Factory.CreateClient();

    await client.PatchAsJsonAsync(
        "/projects/proyecto-atlas/features/authentication-api",
        new UpdateProjectFeatureCommand(
            "Authentication API",
            "Expose login and refresh endpoints.",
            FeatureStatus.InProgress));

    await client.PatchAsJsonAsync(
        "/projects/proyecto-atlas/milestones/mvp-release",
        new UpdateProjectMilestoneCommand(
            "MVP Release",
            "Cerrar la primera entrega publica con onboarding completo.",
            MilestoneStatus.InProgress,
            new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc)));

    await client.PostAsJsonAsync(
        "/projects/proyecto-atlas/documentations",
        new CreateProjectDocumentationCommand(
            "API Overview",
            "# API",
            3,
            DocumentationKind.Note,
            DocumentationStatus.Draft,
            DocumentationArea.Backend));

    await client.PostAsJsonAsync(
        "/projects/proyecto-atlas/documentations/api-overview/relations",
        new CreateDocumentationRelationCommand("getting-started", DocumentationRelationKind.RelatedTo));

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/activity-feed");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement items = jsonDocument.RootElement.GetProperty("items");

    Assert.True(items.GetArrayLength() >= 6);
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "AuditEvent");
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "FeatureUpdated");
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "MilestoneUpdated");
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "DocumentationRelationCreated");
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "FeatureDocumentationLinkCreated");
    Assert.Contains(items.EnumerateArray(), item => item.GetProperty("type").GetString() == "ProjectRelationCreated");
  }

  [Fact]
  public async Task GetProjectActivityFeed_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/activity-feed");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, ProyectoAtlas.Application.Errors.AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }
}
