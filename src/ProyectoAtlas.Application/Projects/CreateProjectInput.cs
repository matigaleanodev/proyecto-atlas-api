namespace ProyectoAtlas.Application.Projects;

public record CreateProjectInput(
    string Title,
    string Description,
    string RepositoryUrl,
    string Color);


