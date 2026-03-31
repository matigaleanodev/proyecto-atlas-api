using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Projects.Update;

public record UpdateProjectLink(
    string Title,
    string Url,
    string Description,
    int SortOrder,
    ProjectLinkKind Kind
    );
