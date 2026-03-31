using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Tests;

[Collection(ApiIntegrationTestSuite.Name)]
public class ProjectDocumentationsApiIntegrationTests(ApiTestWebApplicationFactory factory)
    : ApiIntegrationTestBase(factory), IClassFixture<ApiTestWebApplicationFactory>
{
  private readonly ApiTestWebApplicationFactory _factory = factory;

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnCreatedDocumentation()
  {
    HttpClient client = Factory.CreateClient();
    string suffix = Guid.NewGuid().ToString("N")[..8];
    CreateProjectDocumentationCommand input = new(
        $"Getting Started {suffix}",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend,
        [
          new CreateProjectDocumentationTag("backend"),
          new CreateProjectDocumentationTag("dotnet")
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.NotNull(response.Headers.Location);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.ContentMarkdown, root.GetProperty("contentMarkdown").GetString());
    Assert.Equal(input.SortOrder, root.GetProperty("sortOrder").GetInt32());
    Assert.Equal(input.Kind.ToString(), root.GetProperty("kind").GetString());
    Assert.Equal(input.Status.ToString(), root.GetProperty("status").GetString());
    Assert.Equal(input.Area.ToString(), root.GetProperty("area").GetString());
    Assert.Equal(2, root.GetProperty("tags").GetArrayLength());
    Assert.Equal($"getting-started-{suffix.ToLowerInvariant()}", root.GetProperty("slug").GetString());
    Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnCreatedFaqDocumentation()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        Title: "Common Questions",
        ContentMarkdown: "## Intro",
        SortOrder: 2,
        Kind: DocumentationKind.FAQ,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Product,
        Tags: null,
        FaqItems:
        [
          new CreateProjectDocumentationFaqItem("What is Atlas?", "Atlas is the documentation backend.", 1),
          new CreateProjectDocumentationFaqItem("Who uses it?", "Engineering teams.", 2)
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string postContent = await response.Content.ReadAsStringAsync();
    using JsonDocument postDocument = JsonDocument.Parse(postContent);
    JsonElement postRoot = postDocument.RootElement;

    Assert.Equal("FAQ", postRoot.GetProperty("kind").GetString());
    Assert.Equal(2, postRoot.GetProperty("faqItems").GetArrayLength());

    HttpResponseMessage getResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/common-questions");

    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

    string getContent = await getResponse.Content.ReadAsStringAsync();
    using JsonDocument getDocument = JsonDocument.Parse(getContent);
    JsonElement getRoot = getDocument.RootElement;

    Assert.Equal(2, getRoot.GetProperty("faqItems").GetArrayLength());
    string[] questions = getRoot.GetProperty("faqItems")
        .EnumerateArray()
        .Select(item => item.GetProperty("question").GetString() ?? string.Empty)
        .OrderBy(question => question, StringComparer.Ordinal)
        .ToArray();

    Assert.Equal(
        ["What is Atlas?", "Who uses it?"],
        questions);
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Guía API: sección / inicial",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("guia-api-seccion-inicial", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/missing-project/documentations", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnConflict_WhenSlugAlreadyExistsWithinProject()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Duplicate",
        3,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.DocumentationSlugConflict, "Documentation slug");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnCreated_WhenSlugExistsInAnotherProject()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Atlas Docs",
        2,
        DocumentationKind.Note,
        DocumentationStatus.Published,
        DocumentationArea.Backend);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/atlas-docs/documentations", input);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnValidationError_WhenKindIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    object input = new
    {
      title = "Getting Started",
      contentMarkdown = "# Proyecto Atlas",
      sortOrder = 1,
      kind = "InvalidKind",
      status = "Draft",
      area = "Backend"
    };

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnValidationError_WhenStatusIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    object input = new
    {
      title = "Getting Started",
      contentMarkdown = "# Proyecto Atlas",
      sortOrder = 1,
      kind = "Page",
      status = "InvalidStatus",
      area = "Backend"
    };

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnValidationError_WhenAreaIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    object input = new
    {
      title = "Getting Started",
      contentMarkdown = "# Proyecto Atlas",
      sortOrder = 1,
      kind = "Page",
      status = "Draft",
      area = "InvalidArea"
    };

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenDecisionTitleConventionIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Architecture decision without ADR prefix",
        "# Proyecto Atlas",
        1,
        DocumentationKind.Decision,
        DocumentationStatus.Draft,
        DocumentationArea.Architecture);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTitleConventionInvalid,
        "ADR-XXX");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenFaqHasNoItems()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        "Common Questions",
        "## Intro",
        2,
        DocumentationKind.FAQ,
        DocumentationStatus.Draft,
        DocumentationArea.Product,
        null,
        []);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationFaqItemsInvalid,
        "FAQ");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenFaqItemSortOrderIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        Title: "Common Questions",
        ContentMarkdown: "## Intro",
        SortOrder: 2,
        Kind: DocumentationKind.FAQ,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Product,
        Tags: null,
        FaqItems:
        [
          new CreateProjectDocumentationFaqItem("What is Atlas?", "Atlas is the documentation backend.", 0)
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationFaqItemsInvalid,
        "sort order");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenNonFaqIncludesFaqItems()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        Title: "Getting Started Extended",
        ContentMarkdown: "# Intro",
        SortOrder: 2,
        Kind: DocumentationKind.Page,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags: null,
        FaqItems:
        [
          new CreateProjectDocumentationFaqItem("What is Atlas?", "Atlas is the documentation backend.", 1)
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationFaqItemsInvalid,
        "Only FAQ");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenTagNameIsEmpty()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        Title: "Getting Started With Tags",
        ContentMarkdown: "# Intro",
        SortOrder: 2,
        Kind: DocumentationKind.Note,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags:
        [
          new CreateProjectDocumentationTag(" ")
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTagsInvalid,
        "non-empty");
  }

  [Fact]
  public async Task PostProjectDocumentations_ShouldReturnBadRequest_WhenTagsAreDuplicated()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand input = new(
        Title: "Getting Started With Tags",
        ContentMarkdown: "# Intro",
        SortOrder: 2,
        Kind: DocumentationKind.Note,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags:
        [
          new CreateProjectDocumentationTag("Node"),
          new CreateProjectDocumentationTag(" node ")
        ]);

    HttpResponseMessage response = await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTagsInvalid,
        "duplicate");
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldReturnPagedDocumentations()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?page=1&pageSize=1");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(1, root.GetProperty("page").GetInt32());
    Assert.Equal(1, root.GetProperty("pageSize").GetInt32());
    Assert.Equal(2, root.GetProperty("totalItems").GetInt32());
    Assert.Equal(2, root.GetProperty("totalPages").GetInt32());
    JsonElement item = Assert.Single(root.GetProperty("items").EnumerateArray());
    Assert.Equal("Page", item.GetProperty("kind").GetString());
    Assert.Equal("Draft", item.GetProperty("status").GetString());
    Assert.Equal("Backend", item.GetProperty("area").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByQuery()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?query=arch");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Decision", items[0].GetProperty("kind").GetString());
    Assert.Equal("Published", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByKind()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?kind=Decision");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Decision", items[0].GetProperty("kind").GetString());
    Assert.Equal("Published", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByStatus()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations?status=Draft");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("Getting Started", items[0].GetProperty("title").GetString());
    Assert.Equal("Page", items[0].GetProperty("kind").GetString());
    Assert.Equal("Draft", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByKindAndStatus()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations?kind=Decision&status=Published");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Decision", items[0].GetProperty("kind").GetString());
    Assert.Equal("Published", items[0].GetProperty("status").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByArea()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations?area=Architecture");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal("Architecture", items[0].GetProperty("area").GetString());
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldFilterByTag()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response =
        await client.GetAsync("/projects/proyecto-atlas/documentations?tag=architecture");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;
    JsonElement items = root.GetProperty("items");

    Assert.Equal(1, items.GetArrayLength());
    Assert.Equal("ADR-001 Architecture", items[0].GetProperty("title").GetString());
    Assert.Equal(2, items[0].GetProperty("tags").GetArrayLength());
  }

  [Theory]
  [InlineData("/projects/proyecto-atlas/documentations?kind=InvalidKind")]
  [InlineData("/projects/proyecto-atlas/documentations?status=InvalidStatus")]
  [InlineData("/projects/proyecto-atlas/documentations?area=InvalidArea")]
  public async Task GetProjectDocumentations_ShouldReturnValidationError_WhenFilterIsInvalid(string path)
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync(path);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.BadRequest, AtlasErrorCodes.ValidationError, "invalid");
  }

  [Fact]
  public async Task GetProjectDocumentations_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/documentations");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnDocumentation_WhenDocumentationExists()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal("getting-started", root.GetProperty("slug").GetString());
    Assert.Equal("Getting Started", root.GetProperty("title").GetString());
    Assert.Equal("Page", root.GetProperty("kind").GetString());
    Assert.Equal("Draft", root.GetProperty("status").GetString());
    Assert.Equal("Backend", root.GetProperty("area").GetString());
    Assert.Equal(2, root.GetProperty("tags").GetArrayLength());
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnNotFound_WhenProjectDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/missing-project/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.ProjectNotFound, "Project with slug");
  }

  [Fact]
  public async Task GetProjectDocumentationBySlug_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/projects/proyecto-atlas/documentations/missing-doc");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldUpdateDocumentation_WhenDocumentationExists()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Published);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

    string content = await response.Content.ReadAsStringAsync();
    using JsonDocument jsonDocument = JsonDocument.Parse(content);
    JsonElement root = jsonDocument.RootElement;

    Assert.Equal(input.Title, root.GetProperty("title").GetString());
    Assert.Equal(input.ContentMarkdown, root.GetProperty("contentMarkdown").GetString());
    Assert.Equal(input.SortOrder, root.GetProperty("sortOrder").GetInt32());
    Assert.Equal("Page", root.GetProperty("kind").GetString());
    Assert.Equal(input.Status.ToString(), root.GetProperty("status").GetString());
    Assert.Equal(2, root.GetProperty("tags").GetArrayLength());
    Assert.Equal("quick-start", root.GetProperty("slug").GetString());
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReplaceFaqItems_WhenDocumentationIsFaq()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand createInput = new(
        Title: "Common Questions",
        ContentMarkdown: "## Intro",
        SortOrder: 2,
        Kind: DocumentationKind.FAQ,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Product,
        Tags: null,
        FaqItems:
        [
          new CreateProjectDocumentationFaqItem("Old question", "Old answer", 1)
        ]);

    HttpResponseMessage createResponse =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", createInput);

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    UpdateProjectDocumentationCommand updateInput = new(
        "Common Questions",
        "## Updated",
        2,
        DocumentationStatus.Published,
        [
          new UpdateProjectDocumentationFaqItem("What is Atlas?", "Atlas is the documentation backend.", 1),
          new UpdateProjectDocumentationFaqItem("Who uses it?", "Engineering teams.", 2)
        ]);

    HttpResponseMessage patchResponse =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/common-questions", updateInput);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    string patchContent = await patchResponse.Content.ReadAsStringAsync();
    using JsonDocument patchDocument = JsonDocument.Parse(patchContent);
    JsonElement patchRoot = patchDocument.RootElement;

    Assert.Equal(2, patchRoot.GetProperty("faqItems").GetArrayLength());
    Assert.Equal("What is Atlas?", patchRoot.GetProperty("faqItems")[0].GetProperty("question").GetString());
    Assert.Equal("Engineering teams.", patchRoot.GetProperty("faqItems")[1].GetProperty("answer").GetString());
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReplaceTags_WhenTagsAreProvided()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand createInput = new(
        Title: "Getting Started With Tags",
        ContentMarkdown: "# Intro",
        SortOrder: 2,
        Kind: DocumentationKind.Note,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags:
        [
          new CreateProjectDocumentationTag("backend"),
          new CreateProjectDocumentationTag("api")
        ]);

    HttpResponseMessage createResponse =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", createInput);

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    UpdateProjectDocumentationCommand updateInput = new(
        "Getting Started With Tags",
        "## Updated",
        2,
        DocumentationStatus.Published,
        null,
        [
          new UpdateProjectDocumentationTag("dotnet"),
          new UpdateProjectDocumentationTag("architecture")
        ]);

    HttpResponseMessage patchResponse =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started-with-tags", updateInput);

    Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

    string patchContent = await patchResponse.Content.ReadAsStringAsync();
    using JsonDocument patchDocument = JsonDocument.Parse(patchContent);
    JsonElement patchRoot = patchDocument.RootElement;

    string[] tagNames = patchRoot.GetProperty("tags")
        .EnumerateArray()
        .Select(item => item.GetProperty("name").GetString() ?? string.Empty)
        .OrderBy(name => name, StringComparer.Ordinal)
        .ToArray();

    Assert.Equal(["architecture", "dotnet"], tagNames);
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnBadRequest_WhenTagNameIsEmpty()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Published,
        null,
        [
          new UpdateProjectDocumentationTag(" ")
        ]);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTagsInvalid,
        "non-empty");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnBadRequest_WhenTagsAreDuplicated()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Published,
        null,
        [
          new UpdateProjectDocumentationTag("Node"),
          new UpdateProjectDocumentationTag(" node ")
        ]);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTagsInvalid,
        "duplicate");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnBadRequest_WhenFaqItemSortOrderIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Published,
        [
          new UpdateProjectDocumentationFaqItem("What is Atlas?", "Atlas is the documentation backend.", 0)
        ]);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/getting-started", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationFaqItemsInvalid,
        "sort order");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnBadRequest_WhenDecisionTitleConventionIsInvalid()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Architecture without ADR prefix",
        "## Updated",
        3,
        DocumentationStatus.Published);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/adr-001-architecture", input);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertErrorResponse(
        response,
        HttpStatusCode.BadRequest,
        AtlasErrorCodes.DocumentationTitleConventionInvalid,
        "ADR-XXX");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();
    UpdateProjectDocumentationCommand input = new(
        "Quick Start",
        "## Updated",
        3,
        DocumentationStatus.Draft);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/missing-doc", input);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

  [Fact]
  public async Task PatchProjectDocumentation_ShouldReturnConflict_WhenSlugAlreadyExistsWithinProject()
  {
    HttpClient client = _factory.CreateClient();
    CreateProjectDocumentationCommand createCommand = new(
        "Release Checklist",
        "# Checklist",
        3,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Operations);
    UpdateProjectDocumentationCommand input = new(
        "Getting Started",
        null,
        null,
        null);

    HttpResponseMessage createResponse =
        await client.PostAsJsonAsync("/projects/proyecto-atlas/documentations", createCommand);

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    HttpResponseMessage response =
        await client.PatchAsJsonAsync("/projects/proyecto-atlas/documentations/release-checklist", input);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.Conflict, AtlasErrorCodes.DocumentationSlugConflict, "Documentation slug");
  }

  [Fact]
  public async Task DeleteProjectDocumentation_ShouldReturnNoContent_WhenDocumentationExists()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage deleteResponse =
        await client.DeleteAsync("/projects/proyecto-atlas/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

    HttpResponseMessage getResponse =
        await client.GetAsync("/projects/proyecto-atlas/documentations/getting-started");

    Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteProjectDocumentation_ShouldReturnNotFound_WhenDocumentationDoesNotExist()
  {
    HttpClient client = _factory.CreateClient();

    HttpResponseMessage response =
        await client.DeleteAsync("/projects/proyecto-atlas/documentations/missing-doc");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    await AssertErrorResponse(response, HttpStatusCode.NotFound, AtlasErrorCodes.DocumentationNotFound, "Documentation with slug");
  }

}
