namespace ProyectoAtlas.Application.Features.Projects.Create;

public record CreateProjectCommand(
    string Title,
    string Description,
    string RepositoryUrl,
    string Color,
    IReadOnlyCollection<CreateProjectLink>? Links = null);


