namespace ProyectoAtlas.Application.Projects;

public record ListProjectsInput(
    int Page = 1,
    int PageSize = 10,
    string? Query = null);
