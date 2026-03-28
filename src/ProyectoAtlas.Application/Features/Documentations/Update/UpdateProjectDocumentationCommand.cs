using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.Documentations.Update;

public record UpdateProjectDocumentationCommand(
    string? Title,
    string? ContentMarkdown,
    int? SortOrder,
    DocumentationStatus? Status,
    IReadOnlyCollection<UpdateProjectDocumentationFaqItem>? FaqItems = null,
    IReadOnlyCollection<UpdateProjectDocumentationTag>? Tags = null);
