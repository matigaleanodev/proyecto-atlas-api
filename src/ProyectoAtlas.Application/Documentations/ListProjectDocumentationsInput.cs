namespace ProyectoAtlas.Application.Documentations;

public record ListProjectDocumentationsInput(
    int Page = 1,
    int PageSize = 10,
    string? Query = null);
