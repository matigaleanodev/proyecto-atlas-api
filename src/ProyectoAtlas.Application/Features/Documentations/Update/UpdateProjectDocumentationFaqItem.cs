namespace ProyectoAtlas.Application.Features.Documentations.Update;

public record UpdateProjectDocumentationFaqItem(
    string Question,
    string Answer,
    int SortOrder);
