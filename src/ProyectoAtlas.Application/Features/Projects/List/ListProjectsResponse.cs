using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects;

public record ListProjectsResponse(
    IReadOnlyCollection<Project> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
