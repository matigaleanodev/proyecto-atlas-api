namespace ProyectoAtlas.Application.Projects;

public record CreateProjectCommand(
    string Title,
    string Description,
    string RepositoryUrl,
    string Color);


