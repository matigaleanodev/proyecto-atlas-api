namespace ProyectoAtlas.Application.Projects;

public record UpdateProjectCommand(
      string? Title,
      string? Description,
      string? RepositoryUrl,
      string? Color);
