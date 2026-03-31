namespace ProyectoAtlas.Domain.Projects;

public record ProjectLinkData(string Title, string Url, string Description, int SortOrder, ProjectLinkKind Kind);
