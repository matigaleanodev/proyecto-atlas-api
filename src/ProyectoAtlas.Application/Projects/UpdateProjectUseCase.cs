using ProyectoAtlas.Domain.Projects;
namespace ProyectoAtlas.Application.Projects;

public class UpdateProjectUseCase(IProjectRepository projectRepository)
{
    public async Task<Project> Execute(string slug, UpdateProjectInput input, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);


        var project = await projectRepository.GetBySlug(slug, cancellationToken) ?? throw new KeyNotFoundException($"Project with slug '{slug}' not found."); ;


        project.Update(input.Title, input.Description, input.RepositoryUrl, input.Color);

        await projectRepository.Update(project, cancellationToken);


        return project;
    }
}
