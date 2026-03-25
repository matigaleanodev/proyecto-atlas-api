namespace ProyectoAtlas.Application.Features.Projects.Update;

public record UpdateProjectCommand(
      string? Title,
      string? Description,
      string? RepositoryUrl,
      string? Color);
