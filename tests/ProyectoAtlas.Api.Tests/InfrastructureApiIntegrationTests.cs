namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class InfrastructureApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  [Fact]
  public async Task GetOpenApiDocument_ShouldReturnOk()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/openapi/v1.json");

    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
  }

  [Fact]
  public async Task GetSwaggerUi_ShouldReturnOk()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/swagger/index.html");

    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
  }

  [Fact]
  public async Task GetHealth_ShouldReturnOk()
  {
    HttpClient client = Factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/health");

    Assert.True(response.IsSuccessStatusCode);
    string content = await response.Content.ReadAsStringAsync();
    Assert.Contains("ok", content);
  }
}
