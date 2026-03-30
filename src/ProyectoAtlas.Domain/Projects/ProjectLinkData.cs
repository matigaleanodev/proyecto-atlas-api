namespace ProyectoAtlas.Domain.Projects;

public record ProjectLinkData(string Title, int SortOrder, string Url, ProjectLinkKind Kind, string Description);
