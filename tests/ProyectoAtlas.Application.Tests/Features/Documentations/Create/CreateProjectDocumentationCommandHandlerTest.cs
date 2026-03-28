using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests.Features.Documentations.Create;

public class CreateDocumentationUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentation()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    FakeDocumentationRepository documentationRepository = new();
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.ContentMarkdown, result.ContentMarkdown);
    Assert.Equal(input.SortOrder, result.SortOrder);
    Assert.Equal(input.Kind, result.Kind);
    Assert.Equal(input.Status, result.Status);
    Assert.Equal(input.Area, result.Area);
    Assert.Equal(projectRepository.ProjectBySlug!.Id, result.ProjectId);
    Assert.NotEqual(Guid.Empty, result.Id);
    Assert.Same(result, documentationRepository.AddedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldReturnFaqDocumentation_WhenKindIsFaq()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    FakeDocumentationRepository documentationRepository = new();
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationCommand input = new(
        Title: "Common Questions",
        ContentMarkdown: "## Intro",
        SortOrder: 1,
        Kind: DocumentationKind.FAQ,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Product,
        Tags: null,
        FaqItems:
        [
          new CreateProjectDocumentationFaqItem("What is Atlas?", "Atlas is the documentation backend.", 1),
          new CreateProjectDocumentationFaqItem("Who uses it?", "Engineering teams.", 2)
        ]);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    Assert.Equal(DocumentationKind.FAQ, result.Kind);
    Assert.Equal(2, result.FaqItems.Count);
    Assert.Equal("What is Atlas?", result.FaqItems.First().Question);
  }

  [Fact]
  public async Task Execute_ShouldReturnDocumentationWithTags_WhenTagsAreProvided()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    FakeDocumentationRepository documentationRepository = new();
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationCommand input = new(
        Title: "Getting Started",
        ContentMarkdown: "# Atlas",
        SortOrder: 1,
        Kind: DocumentationKind.Note,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags:
        [
          new CreateProjectDocumentationTag("backend"),
          new CreateProjectDocumentationTag("dotnet")
        ]);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    string[] tagNames = result.Tags
        .Select(tag => tag.Name)
        .OrderBy(name => name, StringComparer.Ordinal)
        .ToArray();

    Assert.Equal(["backend", "dotnet"], tagNames);
  }

  [Fact]
  public async Task Execute_ShouldNormalizeSlug_WhenTitleContainsAccentsAndSymbols()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    FakeDocumentationRepository documentationRepository = new();
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(documentationRepository, projectRepository);
    CreateProjectDocumentationCommand input = new(
        "Guía API: sección / inicial",
        "# Atlas",
        1,
        DocumentationKind.Page,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    Assert.Equal("guia-api-seccion-inicial", result.Slug);
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationTitleConventionException_WhenDecisionTitleIsInvalid()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(
        "Decision without ADR prefix",
        "# Atlas",
        1,
        DocumentationKind.Decision,
        DocumentationStatus.Draft,
        DocumentationArea.Architecture);

    InvalidDocumentationTitleConventionException exception =
        await Assert.ThrowsAsync<InvalidDocumentationTitleConventionException>(() =>
            createDocumentationUseCase.Execute("proyecto-atlas", input));

    Assert.Contains("ADR-XXX", exception.Message, StringComparison.Ordinal);
  }

  [Fact]
  public async Task Execute_ShouldThrowProjectNotFoundException_WhenProjectDoesNotExist()
  {
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    CreateProjectDocumentationCommand input = new(
        "Getting Started",
        "# Atlas",
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
        createDocumentationUseCase.Execute("missing-project", input));
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationFaqItemsException_WhenFaqHasNoItems()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(
        Title: "Common Questions",
        ContentMarkdown: "## Intro",
        SortOrder: 1,
        Kind: DocumentationKind.FAQ,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Product,
        Tags: null,
        FaqItems: []);

    await Assert.ThrowsAsync<InvalidDocumentationFaqItemsException>(() =>
        createDocumentationUseCase.Execute("proyecto-atlas", input));
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationTagsException_WhenTagNameIsEmpty()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(
        Title: "Getting Started",
        ContentMarkdown: "# Atlas",
        SortOrder: 1,
        Kind: DocumentationKind.Note,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags:
        [
          new CreateProjectDocumentationTag(" ")
        ]);

    await Assert.ThrowsAsync<InvalidDocumentationTagsException>(() =>
        createDocumentationUseCase.Execute("proyecto-atlas", input));
  }

  [Fact]
  public async Task Execute_ShouldThrowInvalidDocumentationTagsException_WhenTagsAreDuplicated()
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(
        Title: "Getting Started",
        ContentMarkdown: "# Atlas",
        SortOrder: 1,
        Kind: DocumentationKind.Note,
        Status: DocumentationStatus.Draft,
        Area: DocumentationArea.Backend,
        Tags:
        [
          new CreateProjectDocumentationTag("Node"),
          new CreateProjectDocumentationTag(" node ")
        ]);

    await Assert.ThrowsAsync<InvalidDocumentationTagsException>(() =>
        createDocumentationUseCase.Execute("proyecto-atlas", input));
  }

  [Theory]
  [InlineData(null, "Getting Started", "# Atlas")]
  [InlineData("", "Getting Started", "# Atlas")]
  [InlineData("   ", "Getting Started", "# Atlas")]
  [InlineData("proyecto-atlas", null, "# Atlas")]
  [InlineData("proyecto-atlas", "", "# Atlas")]
  [InlineData("proyecto-atlas", "   ", "# Atlas")]
  [InlineData("proyecto-atlas", "Getting Started", null)]
  [InlineData("proyecto-atlas", "Getting Started", "")]
  [InlineData("proyecto-atlas", "Getting Started", "   ")]
  public async Task Execute_ShouldThrowArgumentException_WhenInputContainsNullOrWhitespace(
      string? projectSlug,
      string? title,
      string? contentMarkdown)
  {
    FakeProjectRepository projectRepository = new()
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateProjectDocumentationCommandHandler createDocumentationUseCase = new(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateProjectDocumentationCommand input = new(
        title!,
        contentMarkdown!,
        1,
        DocumentationKind.Note,
        DocumentationStatus.Draft,
        DocumentationArea.Backend);

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        createDocumentationUseCase.Execute(projectSlug!, input));
  }
}
