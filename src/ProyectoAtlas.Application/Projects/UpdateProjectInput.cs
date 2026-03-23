namespace ProyectoAtlas.Application.Projects;

public record UpdateProjectInput(
      string? Title,
      string? Description,
      string? RepositoryUrl,
      string? Color);
