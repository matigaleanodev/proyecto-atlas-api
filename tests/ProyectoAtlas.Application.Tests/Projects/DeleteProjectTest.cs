using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

public class DeleteProjectUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldDeleteProject_WhenSlugExists()
    {
        var existingProject = new Project(
            "Proyecto Atlas",
            "Backend for project documentation based on markdown",
            "https://github.com/matigaleanodev/proyecto-atlas-api",
            "#1E293B");
        var projectRepository = new FakeProjectRepository
        {
            ProjectBySlug = existingProject
        };
        var useCase = new DeleteProjectUseCase(projectRepository);

        var result = await useCase.Execute("proyecto-atlas");

        Assert.Same(existingProject, result);
        Assert.Same(existingProject, projectRepository.DeletedProject);
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
    {
        var useCase = new DeleteProjectUseCase(new FakeProjectRepository());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.Execute("missing-project"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Execute_ShouldThrowArgumentException_WhenSlugIsInvalid(string? slug)
    {
        var useCase = new DeleteProjectUseCase(new FakeProjectRepository());

        await Assert.ThrowsAnyAsync<ArgumentException>(() => useCase.Execute(slug!));
    }
}
