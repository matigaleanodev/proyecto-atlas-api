using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class CreateDocumentationUseCaseTests
{
  [Fact]
  public async Task Execute_ShouldReturnDocumentation()
  {
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    FakeDocumentationRepository documentationRepository = new FakeDocumentationRepository();
    CreateDocumentationUseCase createDocumentationUseCase = new CreateDocumentationUseCase(documentationRepository, projectRepository);
    CreateDocumentationInput input = new(
        "Getting Started",
        "# Atlas",
        1);

    Documentation result = await createDocumentationUseCase.Execute("proyecto-atlas", input);

    Assert.Equal(input.Title, result.Title);
    Assert.Equal(input.ContentMarkdown, result.ContentMarkdown);
    Assert.Equal(input.SortOrder, result.SortOrder);
    Assert.Equal(projectRepository.ProjectBySlug!.Id, result.ProjectId);
    Assert.NotEqual(Guid.Empty, result.Id);
    Assert.Same(result, documentationRepository.AddedDocumentation);
  }

  [Fact]
  public async Task Execute_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
  {
    CreateDocumentationUseCase createDocumentationUseCase = new CreateDocumentationUseCase(
        new FakeDocumentationRepository(),
        new FakeProjectRepository());
    CreateDocumentationInput input = new(
        "Getting Started",
        "# Atlas",
        1);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        createDocumentationUseCase.Execute("missing-project", input));
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
    FakeProjectRepository projectRepository = new FakeProjectRepository
    {
      ProjectBySlug = new Project(
          "Proyecto Atlas",
          "Backend for project documentation based on markdown",
          "https://github.com/matigaleanodev/proyecto-atlas-api",
          "#1E293B"),
    };
    CreateDocumentationUseCase createDocumentationUseCase = new CreateDocumentationUseCase(
        new FakeDocumentationRepository(),
        projectRepository);
    CreateDocumentationInput input = new CreateDocumentationInput(title!, contentMarkdown!, 1);

    await Assert.ThrowsAnyAsync<ArgumentException>(() =>
        createDocumentationUseCase.Execute(projectSlug!, input));
  }
}
