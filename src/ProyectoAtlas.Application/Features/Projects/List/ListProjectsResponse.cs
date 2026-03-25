using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.List;

public record ListProjectsResponse(
    IReadOnlyCollection<Project> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
