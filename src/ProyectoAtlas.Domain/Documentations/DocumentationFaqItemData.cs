namespace ProyectoAtlas.Domain.Documentations;

public sealed record DocumentationFaqItemData(
    string Question,
    string Answer,
    int SortOrder);
