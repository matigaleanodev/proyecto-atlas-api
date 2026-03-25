namespace ProyectoAtlas.Application.Features.Projects.List;

public record ListProjectsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Query = null);
