using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Projects;

public record ListProjectsOutput(
    IReadOnlyCollection<Project> Items,
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems);
