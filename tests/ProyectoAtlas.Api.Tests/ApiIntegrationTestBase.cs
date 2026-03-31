using System.Net;
using System.Text.Json;

namespace ProyectoAtlas.Api.Tests;

public abstract class ApiIntegrationTestBase(ApiTestWebApplicationFactory factory) : IAsyncLifetime
{
  protected ApiTestWebApplicationFactory Factory { get; } = factory;

  public async Task InitializeAsync()
  {
    await Factory.ResetDatabaseAsync();
    await Factory.SeedProjectsAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  protected static async Task AssertErrorResponse(
      HttpResponseMessage response,
      HttpStatusCode expectedStatusCode,
      string expectedCode,
      string expectedMessageFragment)
  {
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal((int)expectedStatusCode, root.GetProperty("statusCode").GetInt32());
    Assert.Equal(expectedCode, root.GetProperty("code").GetString());

    string message = root.GetProperty("message").GetString() ?? string.Empty;
    Assert.Contains(expectedMessageFragment, message, StringComparison.OrdinalIgnoreCase);
  }
}
