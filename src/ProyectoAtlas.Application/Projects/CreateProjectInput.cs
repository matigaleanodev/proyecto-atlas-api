namespace ProyectoAtlas.Application.Projects.CreateProject;

public record CreateProjectInput(
    string Title,
    string Description,
    string RepositoryUrl,
    string Color);
