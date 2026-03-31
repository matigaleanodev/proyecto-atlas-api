using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Create;

public record CreateProjectLink(
    string Title,
    string Url,
    string Description,
    int SortOrder,
    ProjectLinkKind Kind
    );
